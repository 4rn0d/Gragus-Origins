using UnityEngine;

public class FollowBackground : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float followSpeed = 0.5f; // Lower = slower movement

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void LateUpdate()
    {
        if (player)
        {
            Vector3 targetPosition = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y,
                transform.position.z // keep original Z
            );

            // Lerp towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}