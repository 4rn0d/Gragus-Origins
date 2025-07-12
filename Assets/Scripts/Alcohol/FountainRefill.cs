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
        [SerializeField] private float refillAmount = 4f;
        [SerializeField] private AudioClip refillSound;
        private static PlayerController _playerController;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerController = other.GetComponent<PlayerController>();
                _playerController.triggerActive = true;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerController.triggerActive = false;
                _playerController = null;
                
            }
        }

        public void Activate(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_playerController != null && _playerController.triggerActive)
            {
                transform.parent.gameObject.SetActive(false);
                _playerController.RefillAlcohol(refillAmount);

                SoundFXManager.instance.PlaySoundFXClip(refillSound, transform, 1f);
            }
        }


    }
}