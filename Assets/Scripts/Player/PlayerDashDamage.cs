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
            var controller = GetComponentInParent<Scripts.PlayerController>();
            var enemyHealth = other.GetComponent<Health.EnemyHealth>();

            if (controller != null && controller.IsThePlayerDashing() && enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageToEnemy, player);
                SoundFXManager.instance.PlaySoundFXClip(dashDamageSound, transform, 1f);
                Debug.Log("Player dashed into enemy — enemy took damage.");
            }
        }
    }
}