using UnityEngine;

namespace Map
{
    [RequireComponent(typeof(Collider2D))]
    public class RoomTrigger : MonoBehaviour
    {
        private Room _parentRoom;

        private void Awake()
        {
            _parentRoom = GetComponentInParent<Room>();
            if (_parentRoom == null)
                Debug.LogWarning("RoomTrigger has no parent Room.");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _parentRoom?.OnPlayerEnter();
            }
        }
    }
}