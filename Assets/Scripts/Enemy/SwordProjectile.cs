using Health;
using UnityEngine;

namespace Enemy
{
    public class SwordProjectile : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private float damageToPlayer = 15f;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var healthPlayer = other.GetComponent<Health.PlayerHealth>();
                var playerController = other.GetComponent<Scripts.PlayerController>();

                if (healthPlayer != null && playerController != null && !playerController.IsThePlayerDashing())
                {
                    healthPlayer.TakeDamage(damageToPlayer, playerController);
                }

                Destroy(gameObject);
            }
        }
    }
}