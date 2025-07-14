using UnityEngine;

namespace Enemy
{
    public class StepDetector : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D enemyRb;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private LayerMask groundLayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object is on the Ground layer
            if (((1 << other.gameObject.layer) & groundLayer) != 0)
            {
                Debug.Log("TRIGGER ENTERED: " + other.name);
                Debug.Log("Layer: " + LayerMask.LayerToName(other.gameObject.layer));
                enemyRb.linearVelocity = new Vector2(enemyRb.linearVelocity.x, jumpForce);
                Debug.Log("[StepDetector] Step detected — Jumping");
            }
        }
    }
}