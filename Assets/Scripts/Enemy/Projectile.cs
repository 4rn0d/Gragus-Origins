using System;
using Managers;
using UnityEngine;

namespace Enemy
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private AudioClip dartTravelSound;
        [SerializeField] private AudioClip dartHitSound;


        private Vector2 direction;

        public void SetDirection(Vector2 dir)
        {
            direction = dir.normalized;
            SoundFXManager.instance.PlaySoundFXClip(dartTravelSound, transform, 1f);
            Destroy(gameObject, lifetime); // Auto-destroy after some time
        }

        void Update()
        {
            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Health.PlayerHealth playerHealth = other.GetComponent<Health.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                SoundFXManager.instance.PlaySoundFXClip(dartHitSound, transform, 1f);
                Destroy(gameObject); // Destroy bullet on hit
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
                SoundFXManager.instance.PlaySoundFXClip(dartHitSound, transform, 1f);
                Destroy(gameObject); // Destroy bullet on hit
        }
    }
}