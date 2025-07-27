using System;
using Health;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(10f);
            //bounce mechanic
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
