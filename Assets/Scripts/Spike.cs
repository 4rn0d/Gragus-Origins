using System;
using Health;
using Scripts;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float bounceForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        playerController.changeCanDashStatus(true);
        playerHealth.TakeDamage(10f);

        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null && collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 normal = contact.normal.normalized;

            Vector2 velocity = rb.linearVelocity;
            velocity -= Vector2.Dot(velocity, normal) * normal;
            rb.linearVelocity = velocity;

            rb.AddForce(-normal * bounceForce, ForceMode2D.Impulse);
        }
    }
}