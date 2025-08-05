using Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Health
{
    public abstract class BaseHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] public float startingHealth = 100f;
        [Header("Events")]
        public float currentHealth { get; protected set; }
        public bool dead { get; protected set; }

        protected virtual void Awake()
        {
            currentHealth = startingHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (dead) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (dead) return;
            dead = true;
        }
    }
}