using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Rooms")]
        public GameObject startRoomPrefab;
        public GameObject finalRoomPrefab;
        public List<GameObject> normalRooms;
        public List<GameObject> specialRooms;

        [Header("Settings")]
        public int normalRoomCount = 6;
        public int specialRoomCount = 2;

        private List<Room> placedRooms = new();

        void Start()
        {
            GenerateDungeon();
        }

        void GenerateDungeon()
        {
            placedRooms.Clear();

            // Start Room
            GameObject startGO = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            Room startRoom = startGO.GetComponent<Room>();
            placedRooms.Add(startRoom);

            Queue<Room> frontier = new();
            frontier.Enqueue(startRoom);

            int placedNormals = 0;
            int placedSpecials = 0;

            while (frontier.Count > 0 && (placedNormals < normalRoomCount || placedSpecials < specialRoomCount))
            {
                Room current = frontier.Dequeue();

                foreach (var door in current.doors)
                {
                    if (door.isUsed) continue;

                    GameObject roomPrefab = null;

                    if (placedNormals < normalRoomCount)
                    {
                        roomPrefab = normalRooms[Random.Range(0, normalRooms.Count)];
                        placedNormals++;
                    }
                    else if (placedSpecials < specialRoomCount)
                    {
                        roomPrefab = specialRooms[Random.Range(0, specialRooms.Count)];
                        placedSpecials++;
                    }

                    if (roomPrefab == null) continue;

                    GameObject newGO = Instantiate(roomPrefab);
                    Room newRoom = newGO.GetComponent<Room>();

                    Room.Door matchingDoor = FindMatchingDoor(newRoom, door.direction.Opposite());
                    if (matchingDoor == null)
                    {
                        Destroy(newGO);
                        continue;
                    }

                    // Align doors
                    Vector3 offset = door.doorTransform.position - matchingDoor.doorTransform.localPosition;
                    newGO.transform.position = offset;

                    Bounds newBounds = newRoom.GetBounds();
                    if (IsOverlapping(newBounds))
                    {
                        Destroy(newGO);
                        continue;
                    }

                    door.isUsed = true;
                    matchingDoor.isUsed = true;
                    placedRooms.Add(newRoom);
                    frontier.Enqueue(newRoom);
                }
            }

            foreach (var room in placedRooms)
            {
                foreach (var door in room.doors)
                {
                    if (door.isUsed) continue;

                    GameObject finalGO = Instantiate(finalRoomPrefab);
                    Room finalRoom = finalGO.GetComponent<Room>();
                    Room.Door finalDoor = FindMatchingDoor(finalRoom, door.direction.Opposite());

                    if (finalDoor == null)
                    {
                        Destroy(finalGO);
                        continue;
                    }

                    Vector3 offset = door.doorTransform.position - finalDoor.doorTransform.localPosition;
                    finalGO.transform.position = offset;

                    if (!IsOverlapping(finalRoom.GetBounds()))
                    {
                        door.isUsed = true;
                        finalDoor.isUsed = true;
                        placedRooms.Add(finalRoom);
                        return;
                    }

                    Destroy(finalGO);
                }
            }

            Debug.LogWarning("Could not place final room.");
        }

        Room.Door FindMatchingDoor(Room room, Direction requiredDir)
        {
            foreach (var door in room.doors)
                if (!door.isUsed && door.direction == requiredDir)
                    return door;
            return null;
        }

        bool IsOverlapping(Bounds newRoomBounds)
        {
            foreach (var room in placedRooms)
            {
                if (room.GetBounds().Intersects(newRoomBounds))
                    return true;
            }
            return false;
        }
    }
}
