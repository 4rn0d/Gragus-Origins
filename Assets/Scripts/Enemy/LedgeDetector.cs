using UnityEngine;

namespace Enemy
{
    public class LedgeDetector : MonoBehaviour
    {
        [Header("Ground & Wall Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckDistance = 1.0f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Jump Settings")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float wallCheckDistance = 0.5f;
        [SerializeField] private float jumpCooldown = 1f;
        private float lastJumpTime = -999f;

        public bool IsLedgeAhead(float direction)
        {
            Vector2 origin = groundCheck.position + Vector3.right * direction * 0.5f;
            Debug.DrawRay(origin, Vector2.down * groundCheckDistance, Color.red);
            return !Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        }

        public bool IsWallAhead(float direction)
        {
            Vector2 origin = groundCheck.position;
            Debug.DrawRay(origin, Vector2.right * direction * wallCheckDistance, Color.yellow);
            return Physics2D.Raycast(origin, Vector2.right * direction, wallCheckDistance, obstacleLayer);
        }

        public bool CanJumpOverObstacle(float direction)
        {
            if (Time.time - lastJumpTime < jumpCooldown)
                return false;

            // Check if wall is in front
            Vector2 wallOrigin = groundCheck.position;
            bool wallDetected = Physics2D.Raycast(wallOrigin, Vector2.right * direction, wallCheckDistance, obstacleLayer);

            // Check if there’s ground past the obstacle
            Vector2 ledgeOrigin = groundCheck.position + Vector3.right * direction * 1f;
            bool groundAfterWall = Physics2D.Raycast(ledgeOrigin, Vector2.down, groundCheckDistance, groundLayer);

            return wallDetected && groundAfterWall;
        }

        public void Jump()
        {
            lastJumpTime = Time.time;
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Cancel any downward force
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

            Vector3 forwardOffset = Vector3.right * 0.5f;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(groundCheck.position + forwardOffset, groundCheck.position + forwardOffset + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(groundCheck.position - forwardOffset, groundCheck.position - forwardOffset + Vector3.down * groundCheckDistance);
        }
    }
}
