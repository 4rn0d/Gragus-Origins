using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Player")]
        public GameObject playerPrefab;
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
        public int normalRoomCount = 6;
        public int specialRoomCount = 2;

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

            while (!success && attempt < maxRetries)
            {
                Random.InitState(baseSeed + attempt);

                Shuffle(normalRooms);
                Shuffle(specialRooms);

                GenerateDungeon();

                success = TryPlaceFinalRoom();

                if (!success)
                {
                    ClearDungeon();
                    yield return null;
                    attempt++;
                }
                else
                {
                    Debug.Log("Generated a dungeon with a final room after " + attempt + " attempts.");
                    SpawnPlayerInStartRoom();
                }
            }

            if (!success)
                Debug.LogError("Failed to generate a dungeon with a final room after " + maxRetries + " attempts.");
        }
        void SpawnPlayerInStartRoom()
        {
            Room startRoom = _placedRooms[0]; // Assuming the first room is always the start room
            if (startRoom == null)
            {
                Debug.LogError("Start room is missing!");
                return;
            }

            Vector3 spawnPosition = startRoom.transform.position;

            if (_playerInstance != null)
                Destroy(_playerInstance);

            _playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }

        void ClearDungeon()
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


        void GenerateDungeon()
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
                            frontier.Add(newRoom);
                            _placedRooms.Add(newRoom);
                            door.isUsed = true;

                            Room.Door matching = FindMatchingDoor(newRoom, door.direction.Opposite());
                            if (matching != null)
                                matching.isUsed = true;

                            if (placedNormals < normalRoomCount) placedNormals++;
                            else placedSpecials++;

                            roomPlaced = true;
                            break;
                        }
                    }
                }

                if (!roomPlaced)
                    Debug.Log("Aucune salle ajoutée depuis : " + current.name);
            }
        }

        Room GenerateStartRoom()
        {
            GameObject go = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            go.transform.position = Vector3.zero; 
            Room room = go.GetComponent<Room>();
            _placedRooms.Add(room);
            MarkGridOccupied(room);
            return room;
        }

        GameObject SelectRoomPrefab(int normals, int specials)
        {
            if (normals < normalRoomCount)
                return normalRooms[Random.Range(0, normalRooms.Count)];
            else if (specials < specialRoomCount)
                return specialRooms[Random.Range(0, specialRooms.Count)];
            return null;
        }

        bool TryPlaceFinalRoom()
        {
            List<Room> candidates = new List<Room> { _placedRooms[^1] };
            
            candidates.AddRange(ShuffleList(_placedRooms));

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
        
        bool TryPlaceRoom(GameObject prefab, Room.Door targetDoor, out Room placedRoom)
        {
            placedRoom = null;

            // Step 1: Create room at origin (so door localPosition is valid)
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Room room = go.GetComponent<Room>();

            // Step 2: Find a door on this room facing opposite of targetDoor
            Room.Door matching = FindMatchingDoor(room, targetDoor.direction.Opposite());
            if (matching == null)
            {
                Destroy(go);
                return false;
            }

            // Step 3: Get matching door's position *relative to its room*
            Vector3 matchLocalOffset = matching.doorTransform.localPosition;

            // Step 4: Move the room so its matching door aligns with the target door
            Vector3 targetPos = targetDoor.doorTransform.position;
            go.transform.position = targetPos - matchLocalOffset;

            // Step 5: Snap to grid
            go.transform.position = new Vector3(
                Mathf.Round(go.transform.position.x),
                Mathf.Round(go.transform.position.y),
                Mathf.Round(go.transform.position.z)
            );

            // Step 6: Collision check
            Physics2D.SyncTransforms();
            if (IsOverlapping(room))
            {
                Destroy(go);
                return false;
            }

            // Success
            placedRoom = room;
            return true;
        }


        Room.Door FindMatchingDoor(Room room, Direction direction)
        {
            foreach (var door in room.doors)
                if (!door.isUsed && door.direction == direction)
                    return door;
            return null;
        }

        bool IsOverlapping(Room newRoom)
        {
            foreach (var col in newRoom.colliders)
            {
                // Slightly shrink bounds to avoid edge contact
                Bounds bounds = col.bounds;
                bounds.Expand(-0.1f);

                Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
                foreach (var hit in hits)
                {
                    if (hit.transform != newRoom.transform && hit.transform.root != newRoom.transform)
                    {
                        Debug.Log($"Overlap with {hit.name}");
                        return true;
                    }
                }
            }

            return false;
        }


        bool IsInOccupiedGrid(Room room)
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

        void MarkGridOccupied(Room room)
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

        Vector2Int WorldToGrid(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / GridSize),
                Mathf.FloorToInt(pos.y / GridSize)
            );
        }

        Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / GridSize) * GridSize,
                Mathf.Round(pos.y / GridSize) * GridSize,
                0f
            );
        }

        void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        List<T> ShuffleList<T>(List<T> list)
        {
            List<T> copy = new List<T>(list);
            Shuffle(copy);
            return copy;
        }
        
        GameObject GetFinalRoomPrefabForDirection(Direction doorDir)
        {
            switch (doorDir)
            {
                case Direction.North:
                    return finalRoomDownPrefab;    // Because doors connect opposite
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
    }
}
