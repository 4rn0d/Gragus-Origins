using Managers;
using Scripts;
using UnityEngine;

namespace Alcohol
{
    public class FountainRefill : MonoBehaviour
    {
        [SerializeField] private float refillAmount = 10f;
        [SerializeField] private AudioClip refillSound;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.RefillAlcohol(refillAmount);

                    SoundFXManager.instance.PlaySoundFXClip(refillSound, transform, 1f);
                }
            }
        }
    }
}