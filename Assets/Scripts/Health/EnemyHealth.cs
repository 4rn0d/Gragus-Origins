using UnityEngine;
using Map;
using Scripts;
using UnityEngine.UI;

namespace Health
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField] private Behaviour[] componentsToDisable;

        private Image _healthBar;


        public override void TakeDamage(float damage)
        {
            // Check if the object is on the "Boss" layer
            if (gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                // Adjust the boss health bar here
                if (_healthBar == null)
                {
                    GameObject bar = GameObject.FindWithTag("BossHealthbar");
                    if (bar != null)
                        _healthBar = bar.GetComponent<Image>();
                }

                if (_healthBar != null)
                    _healthBar.fillAmount = currentHealth / startingHealth;
                Debug.Log("Boss is taking damage — update boss health bar");
            }
            else
            {
                // Adjust regular enemy health bar if needed
                Debug.Log("Regular enemy is taking damage");
            }

            base.TakeDamage(damage);
        }

        protected override void Die()
        {
            if (dead) return;

            dead = true;
            Debug.Log("Enemy died.");

            foreach (var comp in componentsToDisable)
                comp.enabled = false;

            // Notify parent room if needed
            Room room = GetComponentInParent<Room>();
            if (room != null)
            {
                GameObject enemyRoot = transform.parent != null ? transform.parent.gameObject : gameObject;
                room.OnEnemyDied(enemyRoot);
                Destroy(enemyRoot);
            }
            else
            {
                if (gameObject.layer == LayerMask.NameToLayer("Boss"))
                {
                    Destroy(_healthBar.gameObject);
                }

                Destroy(gameObject);
            }
        }
    }
}