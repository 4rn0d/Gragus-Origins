using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Health
{
    public class PlayerHealth : BaseHealth
    {
        [Header("iFrames")]
        [SerializeField] private float iFramesDuration = 1f;
        [SerializeField] private int numberOfFlashes = 4;
        [SerializeField] private SpriteRenderer spriteRend;

        [Header("Components")]
        [SerializeField] private Behaviour[] componentsToDisable;
        private Image _healthBar;
        public bool invulnerable { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (spriteRend == null)
                spriteRend = GetComponent<SpriteRenderer>();

            _healthBar = GameObject.FindWithTag("HealthBar")?.GetComponent<Image>();
        }

        public override void TakeDamage(float damage)
        {
            if (invulnerable || dead) return;
        
            Debug.Log("Player is taking damage");
            
            base.TakeDamage(damage);

            if (!dead)
            {
                StartCoroutine(Invulnerability());
            }

            if (_healthBar != null)
                _healthBar.fillAmount = currentHealth / startingHealth;
        }

        private IEnumerator Invulnerability()
        {
            invulnerable = true;
            Physics2D.IgnoreLayerCollision(10, 11, true);

            for (int i = 0; i < numberOfFlashes; i++)
            {
                spriteRend.color = new Color(1, 0, 0, 0.5f);
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                spriteRend.color = Color.white;
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            }

            Physics2D.IgnoreLayerCollision(10, 11, false);
            invulnerable = false;
        }

        protected override void Die()
        {
            if (dead) return;

            dead = true;
            foreach (var comp in componentsToDisable)
                comp.enabled = false;

            Debug.Log("Player died — returning to main menu.");
            SceneManager.LoadScene(0); // Main menu scene index
        }
    }
}
