using System;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    
    [SerializeField] Rigidbody2D rb;

    private bool _isRotating;

    private void Update()
    {
        if (_isRotating)
        {
            transform.Rotate(0,0,-0.2f); 
        }
        else
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public void InitalizeBarrel(Transform launchOffset, float speed)
    {
        _isRotating = true;
        rb.linearVelocity = transform.right * speed;
    }
    
    public void StopBarrel()
    {
        rb.linearVelocity = Vector2.zero;
        _isRotating = false;
    }

    public void DestroyBarrel()
    {
        Destroy(gameObject);
    }
    
}
