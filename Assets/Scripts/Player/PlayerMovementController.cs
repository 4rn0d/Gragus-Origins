using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float jumpMovementReductionForce = 0.14f;
    [SerializeField] float minJumpVelocity = 5f;
    [SerializeField] float acceleration;
    [SerializeField] float decceleration;
    [SerializeField] float velPower;
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 7.5f;
    [SerializeField] float dashDuration = 0.4f;
    [SerializeField] float bounceVerticalBoost = 4f;
    [SerializeField] float bounceHorizontalForce = 6f;
    [SerializeField] float bounceInputLockDuration = 0.6f;

    [Header("Air Control")]
    [SerializeField] float fallAirControlMinMultiplier = 0.3f;
    [SerializeField] float fallAirControlMaxMultiplier = 1f;
    [SerializeField] float fallSpeedForMinControl = -10f;

    [Header("Other")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    private float _horizontal;
    private float _lastNonZeroHorizontal = 1;

    private bool _isDashing;
    private float _dashTimer;
    private float _dashDirection;
    private bool _canDash = true;


    private bool _justBounced;
    private bool _isBouncing;
    private float _bounceTimer;
    private bool _hasBouncedThisDash;
    private float _rawHorizontalInput;


    private void FixedUpdate()
    {
        HandleMovement();
        HandleDash();
        HandleBounceTimer();
        UpdateFacingDirection();
        _justBounced = false;
    }

    private void HandleMovement()
    {
        
        animator.SetFloat("Speed", Mathf.Abs(_horizontal));
        
        if (_isDashing || _isBouncing) return;

        float fallSpeed = rb.linearVelocity.y;
        float airControlMultiplier = 1f;

        if (!IsGrounded() && fallSpeed < 0)
        {
            float t = Mathf.InverseLerp(0f, fallSpeedForMinControl, fallSpeed);
            airControlMultiplier = Mathf.Lerp(fallAirControlMaxMultiplier, fallAirControlMinMultiplier, t);
        }

        if (IsGrounded())
        {
            animator.SetBool("IsJumping", false);
        }

        float targetSpeed = _horizontal * moveSpeed * airControlMultiplier;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) >0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        rb.AddForce(movement * Vector2.right);
    }

    private void HandleDash()
    {
        if (!_isDashing) return;

        rb.linearVelocity = new Vector2(_dashDirection * dashSpeed, 0);
        _dashTimer -= Time.fixedDeltaTime;

        if (!_hasBouncedThisDash && IsTouchingWall())
        {
            TriggerBounce();
        }
        else if (_dashTimer <= 0)
        {
            EndDash();
        }
    }

    private void TriggerBounce()
    {
        _isBouncing = true;
        _bounceTimer = bounceInputLockDuration;
        _isDashing = false;
        _hasBouncedThisDash = true;
        _justBounced = true;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        float bounceDir = -_dashDirection;
        transform.localScale = new Vector3(Mathf.Sign(bounceDir) * -1.5f, 1.5f, 1f);
        rb.linearVelocity = new Vector2(bounceDir * bounceHorizontalForce, bounceVerticalBoost);
        
        animator.SetBool("IsDashing", false);
    }

    private void EndDash()
    {
        animator.SetBool("IsDashing", false);
        _isDashing = false;
        if (_rawHorizontalInput == 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            _horizontal = 0;
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }
    }

    private void HandleBounceTimer()
    {
        if (_isBouncing)
        {
            _bounceTimer -= Time.fixedDeltaTime;
            if (_bounceTimer <= 0)
            {
                _isBouncing = false;
                _horizontal = _rawHorizontalInput;
            }
        }

        if (IsGrounded() && !_isDashing && !_isBouncing)
        {
            _canDash = true;
        }
    }

    private void UpdateFacingDirection()
    {
        if (_isBouncing) return;

        if (_rawHorizontalInput != 0 && !_isDashing)
        {
            transform.localScale = new Vector3(Mathf.Sign(_rawHorizontalInput) * -1.5f, 1.5f, 1f);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.5f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    public bool IsTouchingWall()
    {
        float direction = _isDashing ? _dashDirection : Mathf.Sign(_horizontal);
        return Physics2D.Raycast(wallCheck.position, Vector2.right * direction, 0.3f, wallLayer);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            animator.SetBool("IsJumping", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * jumpMovementReductionForce, jumpForce);
        }

        if (context.canceled && rb.linearVelocity.y > minJumpVelocity)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, minJumpVelocity);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && !_isDashing && !_isBouncing && _canDash)
        {
            animator.SetBool("IsDashing", true);
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashDirection = _lastNonZeroHorizontal;
            _hasBouncedThisDash = false;
            _canDash = false;
            transform.localScale = new Vector3(Mathf.Sign(_dashDirection) * -1.5f, 1.5f, 1f);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<Vector2>().x;
        _rawHorizontalInput = input;

        if (input != 0)
            _lastNonZeroHorizontal = Mathf.Sign(input);

        if (!_isDashing && !_isBouncing)
        {
            _horizontal = input;
        }
    }

}
