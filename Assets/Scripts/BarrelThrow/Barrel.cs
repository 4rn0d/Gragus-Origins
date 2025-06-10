using UnityEngine;

public class Barrel : MonoBehaviour
{
    
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed = 20f;
    
    void Start()
    {
        rb.linearVelocity = transform.right * speed;
    }
}
