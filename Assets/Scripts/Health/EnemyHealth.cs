using UnityEngine;
using Map;
using Scripts;
using UnityEngine.UI;

namespace Health
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField] private Behaviour[] componentsToDisable;

        private SpriteRenderer _spriteRenderer;
        private Image _healthBar;

        protected override void Awake()
        {
            base.Awake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }


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
                StartCoroutine(FlashSprite());
            }

            base.TakeDamage(damage);
        }

        protected override void Die()
        {
            base.Die();

            Debug.Log("Enemy died.");

            foreach (var comp in componentsToDisable)
                comp.enabled = false;

            Room room = GetComponentInParent<Room>();
            GameObject enemyRoot = transform.parent != null ? transform.parent.gameObject : gameObject;

            if (room != null)
            {
                room.OnEnemyDied(enemyRoot);
                Destroy(enemyRoot);
            }
            else
            {
                if (_healthBar != null)
                    Destroy(_healthBar.gameObject);

                Destroy(enemyRoot);
            }
        }

        private System.Collections.IEnumerator FlashSprite()
        {
            if (!_spriteRenderer)
                yield break;

            Color originalColor = _spriteRenderer.color;
            Color flashColor = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white

            int flashCount = 3;
            float flashDuration = 0.1f; // time for each flash on/off

            for (int i = 0; i < flashCount; i++)
            {
                _spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                _spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
        }
    }
}