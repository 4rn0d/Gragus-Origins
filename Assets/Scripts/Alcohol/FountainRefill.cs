using System;
using Managers;
using Map;
using Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Alcohol
{
    public class FountainRefill : Interactable
    {
        [SerializeField] private SpriteRenderer fountainRenderer;
        [SerializeField] private float refillAmount = 4f;
        [SerializeField] private AudioClip refillSound;
        

        

        private void Start()
        {
            fPromptUI.SetActive(false);
            fountainRenderer.color = Color.white;
        }

        public override void Interact(PlayerController player)
        {
            if (_isUsed || player._alcoholBar.fillAmount == 1) return;

            player.RefillAlcohol(refillAmount);
            _isUsed = true;
            fPromptUI.SetActive(false);
            fountainRenderer.color = Color.gray;
            SoundFXManager.instance.PlaySoundFXClip(refillSound, transform, 1f);
        }
    }

}