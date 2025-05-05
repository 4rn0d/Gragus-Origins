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
    
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    private float _horizontal;

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(_horizontal * moveSpeed, rb.linearVelocity.y);
        if (_horizontal < 0)
        {
            gameObject.transform.localScale = new Vector2(0.25f, 0.25f);
        }
        if (_horizontal > 0)
        {
            gameObject.transform.localScale = new Vector2(-0.25f, 0.25f);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
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
    
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
}
