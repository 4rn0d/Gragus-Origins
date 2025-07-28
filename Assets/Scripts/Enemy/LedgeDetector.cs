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
            float rayLength = groundCheckDistance + 0.1f;
            Debug.DrawRay(origin, Vector2.down * rayLength, Color.red);
            return !Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
        }


        public bool IsWallAhead(float direction)
        {
            Vector2 offsetOrigin = groundCheck.position + Vector3.right * direction * 0.4f;
            Debug.DrawRay(offsetOrigin, Vector2.right * direction * wallCheckDistance, Color.yellow);
            return Physics2D.Raycast(offsetOrigin, Vector2.right * direction, wallCheckDistance, obstacleLayer);
        }

        public bool CanJumpOverObstacle(float direction)
        {
            if (Time.time - lastJumpTime < jumpCooldown)
                return false;

            float wallOffset = 0.6f;
            float ledgeOffset = 1.2f;

            Vector2 wallOrigin = groundCheck.position + Vector3.right * direction * wallOffset;
            Vector2 ledgeOrigin = wallOrigin + Vector2.right * direction * ledgeOffset;

            bool wallDetected = Physics2D.Raycast(wallOrigin, Vector2.right * direction, wallCheckDistance, obstacleLayer);
            bool groundAfterWall = Physics2D.Raycast(ledgeOrigin, Vector2.down, groundCheckDistance, groundLayer);

            Debug.DrawRay(wallOrigin, Vector2.right * direction * wallCheckDistance, wallDetected ? Color.red : Color.gray);
            Debug.DrawRay(ledgeOrigin, Vector2.down * groundCheckDistance, groundAfterWall ? Color.green : Color.yellow);

            return wallDetected && groundAfterWall;
        }

        
        public bool CanDropFromLedge(float direction)
        {
            Vector2 origin = groundCheck.position + Vector3.right * direction * 0.5f;
            float dropDistance = groundCheckDistance + 0.5f;
    
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, dropDistance, groundLayer);
            Debug.DrawRay(origin, Vector2.down * dropDistance, hit.collider != null ? Color.green : Color.yellow);
            
            return hit.collider != null;
        }


        public void Jump()
        {
            lastJumpTime = Time.time;
            if (rb != null)
            {
                Debug.Log("[Jump] Jumping!");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("[Jump] Rigidbody is missing!");
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
        public bool IsGrounded()
        {
            float checkDistance = 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

            Debug.DrawRay(groundCheck.position, Vector2.down * checkDistance, hit.collider != null ? Color.green : Color.red);

            return hit.collider != null;
        }

    }
}
