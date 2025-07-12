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
        private static PlayerController _playerController;
        [SerializeField] private bool _isUsed;

        private void Start()
        {
            fPromptUI.SetActive(false);
            _isUsed = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isUsed)
            {
                _playerController = other.GetComponent<PlayerController>();
                _playerController.triggerActive = true;
                fPromptUI.SetActive(true);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerController.triggerActive = false;
                _playerController = null;
                fPromptUI.SetActive(false);
                
            }
        }

        public void Activate(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_playerController != null && _playerController.triggerActive)
            {
                _playerController.triggerActive = false;
                _playerController.RefillAlcohol(refillAmount);
                _isUsed = true;
                SoundFXManager.instance.PlaySoundFXClip(refillSound, transform, 1f);
            }
        }

        private void Update()
        {
            if (_isUsed)
            {
                fountainRenderer.color = Color.gray;  
            }
        }
    }
}