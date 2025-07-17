using Enemy;
using Managers;
using Scripts;
using UnityEngine;


public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionDamage = 25f;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float stickyTime = 10f;
    public PlayerController playerController;
    public bool _sticky = false;
    public bool _powerful = false;
    
    private float _delay = 1f;
    private bool _canKnockback = true;

    private void Start()
    {
        SoundFXManager.instance.PlaySoundFXClip(explosionSound,transform,1f);
        Destroy(gameObject, 1f); // Auto-destroy after 1 second
    }

    private void Update()
    {
        _delay -= Time.deltaTime;
        if (_delay < 0.97f)
        {
            _canKnockback = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (_canKnockback && player != null)
            {
                player.BarrelJump(transform.position);
            }
        }
        else
        {
            // Identifier la cible ennemie
            var enemyHealth = other.GetComponent<Health.EnemyHealth>();
            if (enemyHealth != null)
            {
                if (_sticky)
                {
                    SlowableEnemy slowable = other.GetComponent<SlowableEnemy>();
                    if (slowable != null)
                    {
                        slowable.Slow(0.6f, stickyTime);
                    }
                }
                
                float finalDamage = _powerful ? explosionDamage * 1.5f : explosionDamage;
                
                enemyHealth.TakeDamage(finalDamage, playerController);
                Debug.Log($"[Explosion] {other.name} took {finalDamage} damage.");
            }
        }
    }

}