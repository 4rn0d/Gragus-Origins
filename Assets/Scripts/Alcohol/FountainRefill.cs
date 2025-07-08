using Scripts;
using UnityEngine;

namespace Alcohol
{
    public class FountainRefill : MonoBehaviour
    {
        [SerializeField] private float refillAmount = 10f;
        [SerializeField] private AudioClip refillSound;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("[FountainRefill] No AudioSource found on object.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.RefillAlcohol(refillAmount);

                    if (audioSource != null && refillSound != null)
                    {
                        audioSource.clip = refillSound;
                        audioSource.Play();
                    }
                }
            }
        }
    }
}