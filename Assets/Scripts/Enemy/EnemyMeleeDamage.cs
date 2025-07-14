using Managers;
using Scripts;
using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeDamage : MonoBehaviour
    {
        [SerializeField] private float damageToPlayer = 25f;
        [SerializeField] private AudioClip meleeHitSound;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsThePlayerDashing()) return;

            var playerHealth = other.GetComponent<Health.PlayerHealth>();
            if (playerHealth != null && !playerHealth.invulnerable)
            {
                playerHealth.TakeDamage(damageToPlayer);
                SoundFXManager.instance.PlaySoundFXClip(meleeHitSound, transform, 1f);
                Debug.Log("Enemy hit player — player took damage.");
            }
        }
    }
}