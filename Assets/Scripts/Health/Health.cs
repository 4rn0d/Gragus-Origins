using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Health
{
    public class Health : MonoBehaviour
    {
        [Header ("Health")]
        [SerializeField] public float startingHealth;
        public float currentHealth { get; private set; }
        //private Animator anim;
        private bool dead;

        [Header("iFrames")]
        [SerializeField] private float iFramesDuration;
        [SerializeField] private int numberOfFlashes;
        private SpriteRenderer spriteRend;

        [Header("Components")]
        [SerializeField] private Behaviour[] components;
        private Image _healthBar;
        private bool invulnerable;

        private void Awake()
        {
            currentHealth = startingHealth;
            
            _healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
            //anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }
        public void TakeDamage(float _damage)
        {
            if (invulnerable) return;
            currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
            _healthBar.fillAmount = currentHealth;

            if (currentHealth > 0)
            {
                //anim.SetTrigger("hurt");
                StartCoroutine(Invunerability());
            }
            else
            {
                if (!dead)
                {
                    Debug.Log("Object dead");
                    //anim.SetTrigger("die");

                    // Deactivate all attached component classes
                    foreach (Behaviour component in components)
                        component.enabled = false;

                    // Make player disappear
                    gameObject.SetActive(false);

                    dead = true;
                }
            }
        }

        public void AddHealth(float _value)
        {
            currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        }
        private IEnumerator Invunerability()
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
    }
}