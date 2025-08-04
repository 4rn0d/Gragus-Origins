using Map;

namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum RoomDiscoveryState
    {
        Unseen,
        Seen,
        Visited
    }

    public class MinimapManager : MonoBehaviour
    {
        [Header("References")]
        public GameObject minimapRoomPrefab;
        public RectTransform minimapPanel; // UI parent

        [Header("Colors")]
        public Color seenColor = Color.gray;
        public Color visitedColor = Color.black;
        public Color currentRoomColor = Color.white;

        private Dictionary<Room, MinimapRoom> roomToMinimap = new();
        private Dictionary<Room, RoomDiscoveryState> roomStates = new();

        private Room currentRoom;

        void Update()
        {
            Room playerRoom = GetCurrentRoomFromPlayer();

            if (playerRoom != null && playerRoom != currentRoom)
            {
                currentRoom = playerRoom;
                DiscoverRoom(currentRoom);
                UpdateRoomColors();
            }
        }
        void DiscoverRoom(Room room)
        {
            roomStates[room] = RoomDiscoveryState.Visited;
            roomToMinimap[room].SetVisible(true);

            // Reveal connected rooms
            foreach (var door in room.doors)
            {
                Room connected = door.connectedRoom;
                if (connected != null && !roomStates.ContainsKey(connected))
                {
                    roomStates[connected] = RoomDiscoveryState.Seen;
                    roomToMinimap[connected].SetVisible(true);
                }
            }
        }
        void UpdateRoomColors()
        {
            foreach (var pair in roomToMinimap)
            {
                Room room = pair.Key;
                MinimapRoom mmRoom = pair.Value;

                if (room == currentRoom)
                {
                    mmRoom.SetColor(currentRoomColor);
                }
                else
                {
                    var state = roomStates.TryGetValue(room, out var val) ? val : RoomDiscoveryState.Unseen;

                    if (state == RoomDiscoveryState.Visited)
                        mmRoom.SetColor(visitedColor);
                    else if (state == RoomDiscoveryState.Seen)
                        mmRoom.SetColor(seenColor);
                }
            }
        }
        public void Initialize(List<Room> allRooms)
        {
            roomToMinimap.Clear();
            roomStates.Clear();

            foreach (Room room in allRooms)
            {
                GameObject go = Instantiate(minimapRoomPrefab, minimapPanel);
                MinimapRoom mmRoom = go.GetComponent<MinimapRoom>();
                
                Rect bounds = room.GetBounds();
                go.GetComponent<RectTransform>().anchoredPosition = WorldToMinimap(bounds.position);
                go.GetComponent<RectTransform>().sizeDelta = WorldToMinimap(bounds.size);

                mmRoom.SetRoomType(GetRoomType(room));
                mmRoom.SetVisible(false);

                roomToMinimap[room] = mmRoom;
                roomStates[room] = RoomDiscoveryState.Unseen;
            }
        }
        RoomType GetRoomType(Room room)
        {
            if (room.name.ToLower().Contains("final")) return RoomType.Final;
            if (room.name.ToLower().Contains("special")) return RoomType.Special;
            return RoomType.Normal;
        }

        Vector2 WorldToMinimap(Vector2 worldPos)
        {
            return worldPos * 5f;
        }

        Room GetCurrentRoomFromPlayer()
        {
            var dungeonGen = DungeonManager.Instance.currentDungeon.GetComponent<DungeonGenerator>();
            return dungeonGen._currentPlayerRoom;
        }
    }
}