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

        [Header("Random Seed")]
        public int seed = 0;
        public bool useRandomSeed = true;
        
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

            // Start Room
            GameObject startGO = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity, transform);
            Room startRoom = startGO.GetComponent<Room>();
            placedRooms.Add(startRoom);

            List<Room> frontier = new();
            frontier.Add(startRoom);

            int placedNormals = 0;
            int placedSpecials = 0;

            while (frontier.Count > 0 && (placedNormals < normalRoomCount || placedSpecials < specialRoomCount))
            {
                int index = Random.Range(0, frontier.Count);
                Room current = frontier[index];
                frontier.RemoveAt(index);

                foreach (var door in ShuffleList(current.doors))
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
                    
                    newGO.transform.position = Vector3.zero;
                    
                    Vector3 delta = door.doorTransform.position - matchingDoor.doorTransform.position;
                    newGO.transform.position += delta;

                    Vector3 snappedPos = new Vector3(
                        Mathf.Round(newGO.transform.position.x),
                        Mathf.Round(newGO.transform.position.y),
                        Mathf.Round(newGO.transform.position.z)
                    );
                    newGO.transform.position = snappedPos;


                    Bounds newBounds = newRoom.GetBounds();
                    if (IsOverlapping(newBounds))
                    {
                        Destroy(newGO);
                        continue;
                    }

                    door.isUsed = true;
                    matchingDoor.isUsed = true;
                    placedRooms.Add(newRoom);
                    frontier.Add(newRoom);
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
            newRoomBounds.Expand(-0.1f); // Shrink bounds slightly to allow small gaps/touches

            foreach (var room in placedRooms)
            {
                Bounds existingBounds = room.GetBounds();
                existingBounds.Expand(-0.1f);

                if (existingBounds.Intersects(newRoomBounds))
                    return true;
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
            List<T> shuffled = new List<T>(list);
            for (int i = 0; i < shuffled.Count; i++)
            {
                int rand = Random.Range(i, shuffled.Count);
                (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
            }
            return shuffled;
        }

    }
}
