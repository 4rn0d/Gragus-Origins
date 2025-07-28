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

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
            Debug.DrawRay(origin, Vector2.down * rayLength, hit.collider ? Color.green : Color.red);

            return !hit.collider;
        }
        public bool ShouldJumpOrTurn(float direction, out bool shouldTurn)
        {
            shouldTurn = false;

            float checkDistance = 0.6f;

            Vector2 feetOrigin = groundCheck.position + new Vector3(direction * 0.5f, 0.0f);
            Vector2 headOrigin = groundCheck.position + new Vector3(direction * 0.5f, 1.2f);

            RaycastHit2D hitFeet = Physics2D.Raycast(feetOrigin, Vector2.right * direction, checkDistance, obstacleLayer);
            RaycastHit2D hitHead = Physics2D.Raycast(headOrigin, Vector2.right * direction, checkDistance, obstacleLayer);

            Debug.DrawRay(feetOrigin, Vector2.right * direction * checkDistance, hitFeet.collider ? Color.magenta : Color.green);
            Debug.DrawRay(headOrigin, Vector2.right * direction * checkDistance, hitHead.collider ? Color.red : Color.cyan);

            if (hitFeet.collider != null)
            {
                if (hitHead.collider == null && IsGrounded())
                {
                    return true;
                }
                else if (hitHead.collider != null && IsGrounded())
                {
                    shouldTurn = true;
                    return false;
                }

                return false;
            }

            return false;
        }

        public void Jump()
        {
            if (rb != null && IsGrounded())
            {
                Vector2 velocity = rb.linearVelocity;
                velocity.y = jumpForce;
                rb.linearVelocity = velocity;
            }
        }
        public bool CanDropFromLedge(float direction)
        {
            Vector2 origin = groundCheck.position + Vector3.right * direction * 0.5f;
            float dropDistance = groundCheckDistance + 0.5f;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, dropDistance, groundLayer);
            Debug.DrawRay(origin, Vector2.down * dropDistance, hit.collider ? Color.cyan : Color.magenta);

            return hit.collider != null;
        }

        public bool IsGrounded()
        {
            float checkDistance = 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
            Debug.DrawRay(groundCheck.position, Vector2.down * checkDistance, hit.collider ? Color.green : Color.red);

            return hit.collider;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

            Vector3 offset = Vector3.right * 0.5f;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(groundCheck.position + offset, groundCheck.position + offset + Vector3.down * groundCheckDistance);
            Gizmos.DrawLine(groundCheck.position - offset, groundCheck.position - offset + Vector3.down * groundCheckDistance);
        }
    }
}
