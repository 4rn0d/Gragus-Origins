using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Salles")]
        public GameObject startRoomPrefab;
        public GameObject finalRoomPrefab;
        public List<GameObject> normalRooms;
        public List<GameObject> specialRooms;

        [Header("Paramètres")]
        public int normalRoomCount = 6;
        public int specialRoomCount = 2;

        [Header("Graine aléatoire")]
        public int seed = 0;
        public bool useRandomSeed = true;

        private List<Room> placedRooms = new();
        private HashSet<Vector2Int> occupiedCells = new();
        private const float gridSize = 1f;

        void Start()
        {
            if (useRandomSeed)
                seed = System.DateTime.Now.GetHashCode();

            Random.InitState(seed);
            Shuffle(normalRooms);
            Shuffle(specialRooms);
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            placedRooms.Clear();
            occupiedCells.Clear();

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
                            placedRooms.Add(newRoom);
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

            if (!TryPlaceFinalRoom())
                Debug.LogWarning("Salle finale non placée.");
        }

        Room GenerateStartRoom()
        {
            GameObject go = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            go.transform.position = Vector3.zero; 
            Room room = go.GetComponent<Room>();
            placedRooms.Add(room);
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
            foreach (var room in placedRooms)
            {
                foreach (var door in room.doors)
                {
                    if (door.isUsed) continue;

                    GameObject go = Instantiate(finalRoomPrefab, Vector3.zero, Quaternion.identity);
                    Room finalRoom = go.GetComponent<Room>();
                    Room.Door finalDoor = FindMatchingDoor(finalRoom, door.direction.Opposite());

                    if (finalDoor == null)
                    {
                        Destroy(go);
                        continue;
                    }

                    // Use same logic as TryPlaceRoom()
                    Vector3 finalLocalOffset = finalDoor.doorTransform.localPosition;
                    go.transform.position = door.doorTransform.position - finalLocalOffset;

                    // Snap to grid
                    go.transform.position = new Vector3(
                        Mathf.Round(go.transform.position.x),
                        Mathf.Round(go.transform.position.y),
                        Mathf.Round(go.transform.position.z)
                    );

                    Physics2D.SyncTransforms();

                    if (!IsOverlapping(finalRoom))
                    {
                        door.isUsed = true;
                        finalDoor.isUsed = true;
                        placedRooms.Add(finalRoom);
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
                        if (occupiedCells.Contains(cell))
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
                        occupiedCells.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        Vector2Int WorldToGrid(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / gridSize),
                Mathf.FloorToInt(pos.y / gridSize)
            );
        }

        Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / gridSize) * gridSize,
                Mathf.Round(pos.y / gridSize) * gridSize,
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
    }
}
