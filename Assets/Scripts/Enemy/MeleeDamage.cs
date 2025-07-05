using System;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private float damageToPlayer = 25f;
    [SerializeField] private float damageToEnemy = 25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Try to get the player's controller
        var controller = other.GetComponent<Scripts.PlayerController>();
        var playerHealth = other.GetComponent<Health.Health>();

        if (controller != null && playerHealth != null)
        {
            if (controller.IsThePlayerDashing()) // You’ll need to expose this with a public method
            {
                // Player is dashing → damage the enemy (this GameObject)
                var enemyHealth = GetComponent<Health.Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageToEnemy);
                    Debug.Log("Player dashed into enemy — enemy took damage.");
                }
            }
            else
            {
                // Player is not dashing → damage the player
                playerHealth.TakeDamage(damageToPlayer);
                Debug.Log("Enemy hit player — player took damage.");
            }
        }
    }
}