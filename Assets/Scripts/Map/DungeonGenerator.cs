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
            Room startRoom = GenerateStartRoom();
            List<Room> frontier = new() { startRoom };

            int placedNormals = 0;
            int placedSpecials = 0;

            const int maxAttempts = 5;
            const float branchChance = 0.2f;

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
            Room room = go.GetComponent<Room>();
            placedRooms.Add(room);
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

        bool TryPlaceRoom(GameObject prefab, Room.Door targetDoor, out Room placedRoom)
        {
            placedRoom = null;

            GameObject go = Instantiate(prefab);
            Room room = go.GetComponent<Room>();
            Room.Door matching = FindMatchingDoor(room, targetDoor.direction.Opposite());

            if (matching == null)
            {
                Destroy(go);
                return false;
            }

            go.transform.position = Vector3.zero;
            Vector3 offset = targetDoor.doorTransform.position - matching.doorTransform.position;
            go.transform.position += offset;

            Vector3 snapped = new Vector3(
                Mathf.Round(go.transform.position.x),
                Mathf.Round(go.transform.position.y),
                Mathf.Round(go.transform.position.z)
            );
            go.transform.position = snapped;

            Physics2D.SyncTransforms();

            if (IsOverlapping(room))
            {
                Destroy(go);
                return false;
            }

            placedRoom = room;
            return true;
        }

        bool TryPlaceFinalRoom()
        {
            foreach (var room in placedRooms)
            {
                foreach (var door in room.doors)
                {
                    if (door.isUsed) continue;

                    GameObject go = Instantiate(finalRoomPrefab);
                    Room finalRoom = go.GetComponent<Room>();
                    Room.Door finalDoor = FindMatchingDoor(finalRoom, door.direction.Opposite());

                    if (finalDoor == null)
                    {
                        Destroy(go);
                        continue;
                    }

                    go.transform.position = Vector3.zero;
                    Vector3 offset = door.doorTransform.position - finalDoor.doorTransform.position;
                    go.transform.position += offset;

                    Vector3 snapped = new Vector3(
                        Mathf.Round(go.transform.position.x),
                        Mathf.Round(go.transform.position.y),
                        Mathf.Round(go.transform.position.z)
                    );
                    go.transform.position = snapped;

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

        Room.Door FindMatchingDoor(Room room, Direction direction)
        {
            foreach (var door in room.doors)
                if (!door.isUsed && door.direction == direction)
                    return door;
            return null;
        }

        bool IsOverlapping(Room newRoom)
        {
            float margin = 0.001f; // Réduit légèrement les bounding boxes

            foreach (var other in placedRooms)
            {
                if (other == null || other == newRoom) continue;

                foreach (var newCol in newRoom.colliders)
                {
                    Bounds newBounds = newCol.bounds;
                    newBounds.Expand(-margin);

                    foreach (var existingCol in other.colliders)
                    {
                        Bounds existingBounds = existingCol.bounds;
                        existingBounds.Expand(-margin);

                        if (newBounds.Intersects(existingBounds))
                        {
                            Debug.Log($"Collision détectée entre {newRoom.name} et {other.name}");
                            return true;
                        }
                    }
                }
            }

            return false;
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
