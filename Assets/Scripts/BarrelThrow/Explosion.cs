using Managers;
using Scripts;
using UnityEngine;


public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionDamage = 25f;
    [SerializeField] private AudioClip explosionSound;

    private float _delay = 1f;
    private bool _canKnockback = true;

    private void Start()
    {
        SoundFXManager.instance.PlaySoundFXClip(explosionSound, transform, 1f);

        Destroy(gameObject, 1f); // Auto-destroy after 1 second
    }

    private void Update()
    {
        _delay -= Time.deltaTime;
        if (_delay < 0.97f)
        {
            _canKnockback = false; // Small window for knockback
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (_canKnockback && player != null)
            {
                player.BarrelJump(transform.position);
            }
        }
        else
        {
            Health.EnemyHealth health = other.GetComponent<Health.EnemyHealth>();
            if (health != null)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                health.TakeDamage(explosionDamage, player);
                Debug.Log($"[Explosion] {other.name} took {explosionDamage} damage.");
            }
        }
    }
}