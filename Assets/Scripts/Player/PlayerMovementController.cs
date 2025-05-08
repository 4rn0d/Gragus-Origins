using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable All

public class PlayerController : MonoBehaviour
{

    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    
    [Header("Player Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpMovementReductionForce;
    
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float bounceVerticalBoost;
    
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask groundLayer;

    private float _horizontal;
    private bool _isDashing;
    private float _dashTimer;
    private float _dashDirection;

    private void FixedUpdate()
    {
        if (!_isDashing)
        {
            rb.linearVelocity = new Vector2(_horizontal * moveSpeed, rb.linearVelocity.y);
        }
        if (_horizontal < 0)
        {
            gameObject.transform.localScale = new Vector2(0.75f, 0.75f);
        }
        if (_horizontal > 0)
        {
            gameObject.transform.localScale = new Vector2(-0.75f, 0.75f);
        }
        
        if (_isDashing)
        {
            
            rb.linearVelocity = new Vector2(_dashDirection * dashSpeed, 0);
            if (IsTouchingWall())
            {
                _dashDirection *= -1;
                _horizontal *= -1;;
                
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceVerticalBoost);
                
                _isDashing = false;
            }
            _dashTimer -= Time.fixedDeltaTime;

            if (_dashTimer <= 0)
            {
                _isDashing = false;
                rb.linearVelocity = Vector2.zero;
            }
        }

    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    public bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        Debug.Log(IsGrounded());
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * jumpMovementReductionForce, jumpForce);
        }
    }

    
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDashing)
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashDirection = _horizontal;
        }
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        if (!_isDashing)
        {
            _horizontal = context.ReadValue<Vector2>().x;
        }
    }
}
