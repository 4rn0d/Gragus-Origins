using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    
    [Header("Player Settings")]
    [SerializeField] float moveSpeed;

    private float _horizontal;

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(_horizontal * moveSpeed, rb.linearVelocity.y);
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
}
