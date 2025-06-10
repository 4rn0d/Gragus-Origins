using System;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float deceleration = 2f;
    
    private bool _isStopping = false;
    private bool _isRotating;

    private void Update()
    {
        if (_isRotating)
        {
            transform.Rotate(0,0,-0.2f); 
        }
        else
        {
            transform.Rotate(0,0,0);
        }
    }
    
    private void FixedUpdate()
    {
        if (_isStopping)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * deceleration);

            if (rb.linearVelocity.magnitude < 0.05f)
            {
                rb.linearVelocity = Vector2.zero;
                _isStopping = false;
                _isRotating = false;
            }
        }
    }

    public void InitalizeBarrel(Transform launchOffset, float speed)
    {
        _isRotating = true;
        rb.linearVelocity = transform.right * speed;
    }
    
    public void StopBarrel()
    {
        _isStopping = true;
    }

    public void ExplodeBarrel()
    {
        Destroy(gameObject);
    }
    
}
