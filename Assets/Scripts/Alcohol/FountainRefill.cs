using System;
using Managers;
using Map;
using Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Alcohol
{
    public class FountainRefill : MonoBehaviour
    {
        [SerializeField] private GameObject fPromptUI;
        [SerializeField] private SpriteRenderer fountainRenderer;
        [SerializeField] private float refillAmount = 4f;
        [SerializeField] private AudioClip refillSound;
        private bool _isUsed;

        public bool IsUsed => _isUsed;

        private void Start()
        {
            fPromptUI.SetActive(false);
            fountainRenderer.color = Color.white;
        }

        public void Refill(PlayerController player)
        {
            if (_isUsed) return;

            player.RefillAlcohol(refillAmount);
            _isUsed = true;
            fPromptUI.SetActive(false);
            fountainRenderer.color = Color.gray;
            SoundFXManager.instance.PlaySoundFXClip(refillSound, transform, 1f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isUsed && other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetNearbyFountain(this);
                    fPromptUI.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.ClearNearbyFountain(this);
                    fPromptUI.SetActive(false);
                }
            }
        }
    }

}