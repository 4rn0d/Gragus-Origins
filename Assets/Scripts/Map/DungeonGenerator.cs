using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Player")]
        public GameObject playerPrefab;

        public Vector3 playerOffsetInStartRoom;
        private GameObject _playerInstance;
        
        [Header("Salles")]
        public GameObject startRoomPrefab;
        public GameObject finalRoomUpPrefab;
        public GameObject finalRoomDownPrefab;
        public GameObject finalRoomLeftPrefab;
        public GameObject finalRoomRightPrefab;
        public List<GameObject> normalRooms;
        public List<GameObject> specialRooms;

        [Header("Paramètres")] 
        public int normalRoomCount;

        public int specialRoomCount;

        [Header("Graine aléatoire")]
        public int seed = 0;
        public bool useRandomSeed = true;

        private List<Room> _placedRooms = new();
        private HashSet<Vector2Int> _occupiedCells = new();
        private const float GridSize = 1f;

        private IEnumerator  Start()
        {
            bool success = false;
            int attempt = 0;
            int maxRetries = 50;
            int baseSeed = useRandomSeed ? System.DateTime.Now.GetHashCode() : seed;

            while ((!success && attempt < maxRetries || _placedRooms.Count < normalRoomCount + specialRoomCount + 2) && attempt < maxRetries)
            {
                Random.InitState(baseSeed + attempt);

                Shuffle(normalRooms);
                Shuffle(specialRooms);

                GenerateDungeon();

                success = TryPlaceFinalRoom();
                Debug.Log("Nb Rooms : " + (_placedRooms.Count) + " and should be " + (normalRoomCount + specialRoomCount + 2));
                if (!success || _placedRooms.Count < normalRoomCount + specialRoomCount + 2)
                {
                    ClearDungeon();
                    yield return null;
                    attempt++;
                }
                else
                {
                    Debug.Log("Generated a dungeon with a final room after " + attempt + " attempts.");
                    SpawnPlayerInStartRoom();
                    EnableAllUnusedDoors();
                }
            }

            if (!success)
                Debug.LogError("Failed to generate a dungeon with a final room after " + maxRetries + " attempts.");
        }
        private void SpawnPlayerInStartRoom()
        {
            Room startRoom = _placedRooms[0];
            if (startRoom == null)
            {
                Debug.LogError("Start room is missing!");
                return;
            }

            Vector3 spawnPosition = startRoom.transform.position + playerOffsetInStartRoom;

            if (_playerInstance != null)
                Destroy(_playerInstance);

            _playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }

        private void ClearDungeon()
        {
            foreach (var room in _placedRooms)
                if (room != null && room.gameObject != null)
                    Destroy(room.gameObject);

            foreach (Transform child in transform)
                Destroy(child.gameObject);

            _placedRooms.Clear();
            _occupiedCells.Clear();
            
            Physics2D.SyncTransforms();
        }

        //TODO make it so the special room have X(make it balanced so i correlate to the noumber of room that the whole dungeon has, a dungeon with 5 room might want 50 % and 20 room might want 10% idk) chance to be placed when a branche is created and not always after all the room are placed
        private void GenerateDungeon()
        {
            _placedRooms.Clear();
            _occupiedCells.Clear();

            Room startRoom = GenerateStartRoom();
            List<Room> frontier = new() { startRoom };

            int placedNormals = 0;
            int placedSpecials = 0;

            const int maxAttempts = 5;
            const float branchChance = 0.1f;

            while (frontier.Count > 0 && (placedNormals < normalRoomCount || placedSpecials < specialRoomCount))
            {
                int index = (Random.value < branchChance) ? Random.Range(0, frontier.Count) : frontier.Count - 1;
                Room current = frontier[index];
                frontier.RemoveAt(index);

                bool roomPlaced = false;

                foreach (var door in ShuffleList(current.doors))
                {
                    if (door.isUsed) continue;

                    for (int attempt = 0; attempt < maxAttempts; attempt++)
                    {
                        GameObject prefab = SelectRoomPrefab(placedNormals, placedSpecials);
                        if (prefab == null) break;

                        if (TryPlaceRoom(prefab, door, out Room newRoom))
                        {
                            newRoom.depth = current.depth + 1;
                            frontier.Add(newRoom);
                            _placedRooms.Add(newRoom);
                            door.isUsed = true;

                            Room.Door matching = FindMatchingDoor(newRoom, door.direction.Opposite());
                            if (matching != null)
                                matching.isUsed = true;

                            if (normalRooms.Contains(prefab))
                                placedNormals++;
                            else if (specialRooms.Contains(prefab))
                                placedSpecials++;

                            roomPlaced = true;
                            break;
                        }
                    }
                }

                if (!roomPlaced)
                    Debug.Log("Aucune salle ajoutée depuis : " + current.name);
            }
        }

        private Room GenerateStartRoom()
        {
            GameObject go = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            go.transform.position = Vector3.zero; 
            Room room = go.GetComponent<Room>();
            _placedRooms.Add(room);
            MarkGridOccupied(room);
            return room;
        }

        private GameObject SelectRoomPrefab(int normals, int specials)
        {
            int total = normalRoomCount + specialRoomCount;
            
            if (normals >= normalRoomCount && specials >= specialRoomCount)
                return null;

            float specialRatio = specialRoomCount / (float)total;
            bool chooseSpecial = Random.value < specialRatio;

            if (chooseSpecial && specials < specialRoomCount)
                return specialRooms[Random.Range(0, specialRooms.Count)];
    
            if (normals < normalRoomCount)
                return normalRooms[Random.Range(0, normalRooms.Count)];
            
            if (specials < specialRoomCount)
                return specialRooms[Random.Range(0, specialRooms.Count)];
    
            return null;
        }



        private bool TryPlaceFinalRoom()
        {
            int maxDepth = _placedRooms.Max(room => room.depth);
            List<Room> candidates = _placedRooms.Where(r => r.depth == maxDepth || r.depth == (maxDepth - 1)).ToList();
            
            candidates = ShuffleList(candidates);

            foreach (var room in candidates)
            {
                foreach (var door in ShuffleList(room.doors))
                {
                    if (door.isUsed) continue;

                    GameObject prefab = GetFinalRoomPrefabForDirection(door.direction);
                    if (prefab == null) continue;

                    GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    Room finalRoom = go.GetComponent<Room>();
                    if (finalRoom == null)
                    {
                        Destroy(go);
                        continue;
                    }

                    Room.Door finalDoor = FindMatchingDoor(finalRoom, door.direction.Opposite());
                    if (finalDoor == null)
                    {
                        Destroy(go);
                        continue;
                    }

                    go.transform.position = SnapToGrid(door.doorTransform.position - finalDoor.doorTransform.localPosition);
                    Physics2D.SyncTransforms();

                    if (!IsOverlapping(finalRoom) && !IsInOccupiedGrid(finalRoom))
                    {
                        door.isUsed = true;
                        finalDoor.isUsed = true;
                        finalRoom.transform.SetParent(this.transform);
                        _placedRooms.Add(finalRoom);
                        MarkGridOccupied(finalRoom);
                        return true;
                    }

                    Destroy(go);
                }
            }

            return false;
        }
        
        private bool TryPlaceRoom(GameObject prefab, Room.Door targetDoor, out Room placedRoom)
        {
            placedRoom = null;

            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Room room = go.GetComponent<Room>();

            Room.Door matching = FindMatchingDoor(room, targetDoor.direction.Opposite());
            if (matching == null)
            {
                Destroy(go);
                return false;
            }

            Vector3 matchLocalOffset = matching.doorTransform.localPosition;

            Vector3 targetPos = targetDoor.doorTransform.position;
            go.transform.position = targetPos - matchLocalOffset;

            go.transform.position = new Vector3(
                Mathf.Round(go.transform.position.x),
                Mathf.Round(go.transform.position.y),
                Mathf.Round(go.transform.position.z)
            );

            Physics2D.SyncTransforms();
            if (IsOverlapping(room))
            {
                Destroy(go);
                return false;
            }
            
            placedRoom = room;
            room.transform.SetParent(this.transform);
            return true;
        }


        private static Room.Door FindMatchingDoor(Room room, Direction direction)
        {
            foreach (var door in room.doors)
                if (!door.isUsed && door.direction == direction)
                    return door;
            return null;
        }

        private static bool IsOverlapping(Room newRoom)
        {
            HashSet<Collider2D> doorColliders = new HashSet<Collider2D>();
            foreach (var door in newRoom.doors)
            {
                var col = door.doorTransform.GetComponent<Collider2D>();
                if (col != null)
                    doorColliders.Add(col);
            }

            foreach (var col in newRoom.colliders)
            {
                Bounds bounds = col.bounds;
                bounds.Expand(-0.1f);

                Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
                foreach (var hit in hits)
                {
                    // Ignore self
                    if (hit.transform == newRoom.transform || hit.transform.root == newRoom.transform)
                        continue;

                    // Ignore door colliders
                    if (doorColliders.Contains(hit))
                        continue;

                    // **Ignore CameraBounds or any other colliders on a specific layer or tag**
                    if (hit.gameObject.CompareTag("IgnoreForDungeon"))
                        continue;

                    Debug.Log($"Overlap with {hit.name}");
                    return true;
                }
            }

            return false;
        }

        private bool IsInOccupiedGrid(Room room)
        {
            foreach (var col in room.colliders)
            {
                Bounds b = col.bounds;
                Vector2Int min = WorldToGrid(b.min);
                Vector2Int max = WorldToGrid(b.max);

                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        Vector2Int cell = new(x, y);
                        if (_occupiedCells.Contains(cell))
                            return true;
                    }
                }
            }

            return false;
        }

        private void MarkGridOccupied(Room room)
        {
            foreach (var col in room.colliders)
            {
                Bounds b = col.bounds;
                Vector2Int min = WorldToGrid(b.min);
                Vector2Int max = WorldToGrid(b.max);

                for (int x = min.x; x <= max.x; x++)
                {
                    for (int y = min.y; y <= max.y; y++)
                    {
                        _occupiedCells.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        private static Vector2Int WorldToGrid(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / GridSize),
                Mathf.FloorToInt(pos.y / GridSize)
            );
        }

        private static Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / GridSize) * GridSize,
                Mathf.Round(pos.y / GridSize) * GridSize,
                0f
            );
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        private static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> copy = new List<T>(list);
            Shuffle(copy);
            return copy;
        }
        
        private GameObject GetFinalRoomPrefabForDirection(Direction doorDir)
        {
            switch (doorDir)
            {
                case Direction.North:
                    return finalRoomDownPrefab;
                case Direction.South:
                    return finalRoomUpPrefab;
                case Direction.West:
                    return finalRoomRightPrefab;
                case Direction.East:
                    return finalRoomLeftPrefab;
                default:
                    return null;
            }
        }
        private void EnableAllUnusedDoors()
        {
            foreach (var room in _placedRooms)
            {
                room.EnableUnusedDoorVisuals();
            }
        }

    }
}
