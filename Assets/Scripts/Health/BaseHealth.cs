using Scripts;
using UnityEngine;

namespace Health
{
    public abstract class BaseHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] protected float startingHealth = 100f;
        public float currentHealth { get; protected set; }
        public bool dead { get; protected set; }

        protected virtual void Awake()
        {
            currentHealth = startingHealth;
        }

        public virtual void TakeDamage(float damage,  PlayerController player)
        {
            if (dead) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected abstract void Die();
    }
}