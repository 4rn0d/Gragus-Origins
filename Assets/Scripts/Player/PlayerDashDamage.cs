using System;
using Managers;
using Scripts;
using UnityEngine;

namespace Player
{
    public class PlayerDashDamage : MonoBehaviour
    {
        [SerializeField] private float damageToEnemy = 25f;
        [SerializeField] private AudioClip dashDamageSound;

        private void Awake()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;
            var player = gameObject.GetComponentInParent<PlayerController>();
            var controller = GetComponentInParent<PlayerController>();
            var enemyHealth = other.GetComponent<Health.EnemyHealth>();

            if (controller != null && controller.IsThePlayerDashing() && enemyHealth != null)
            {
                if(player.powerful)
                    enemyHealth.TakeDamage(damageToEnemy * 1.5f);
                else
                    enemyHealth.TakeDamage(damageToEnemy);
                SoundFXManager.instance.PlaySoundFXClip(dashDamageSound, transform, 1f);
                Debug.Log("Player dashed into enemy — enemy took damage.");
            }
        }
    }
}