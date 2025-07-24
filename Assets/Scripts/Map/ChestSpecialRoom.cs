using System.Collections;
using System.Collections.Generic;
using Managers;
using Scripts;
using UnityEngine;

namespace Map
{
    public class ChestSpecialRoom : Chest
    {
        public override void Interact(PlayerController player)
        {
            if (_isUsed) return;
            List<Scripts.Alcohol> potions = player.GetPotions();
            Scripts.Alcohol potionToRefill = GetEmptyPotion(potions);
            if (potionToRefill == null)
            {
                Debug.Log("[Chest] Aucun alcool vide à remplir.");
                return;
            }
            animator.SetTrigger("Open");
            potionToRefill.Refill();
            Sprite fullSprite = potionToRefill.GetSpriteFull();
            _isUsed = true;
            fPromptUI.SetActive(false);
            StartCoroutine(AnimateFloatingPotion(fullSprite));
            SoundFXManager.instance.PlaySoundFXClip(openChestSound, transform, 1f);

            
        }
    }
}
