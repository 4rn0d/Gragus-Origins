using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(Collider2D))]
    public class CameraZone : MonoBehaviour
    {
        public CinemachineCamera virtualCamera;
        public int activePriority = 20;
        public int inactivePriority = 5;

        private Transform player;

        void Start()
        {
            if (virtualCamera != null)
                virtualCamera.Priority = inactivePriority;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;

                // Check if player starts inside the zone
                if (GetComponent<Collider2D>().bounds.Contains(player.position))
                {
                    Debug.Log("Player starts inside camera zone: " + gameObject.name);
                    virtualCamera.Priority = activePriority;
                    virtualCamera.Follow = player;
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && virtualCamera != null)
            {
                virtualCamera.Priority = activePriority;
                virtualCamera.Follow = other.transform;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && virtualCamera != null)
            {
                virtualCamera.Priority = inactivePriority;
                virtualCamera.Follow = null; // Optional: only if you want the camera to stop following
            }
        }
    }
}