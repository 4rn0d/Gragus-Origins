using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using Scripts;
using UnityEngine;

namespace Map
{
    public class ChestFinalRoom : Chest
    {

        public override void Interact(PlayerController player)
        {
            if (_isUsed) return;
            

            List<Scripts.Alcohol> potions = player.GetPotions();
            Scripts.Alcohol potionToRepair = GetBrokenPotion(potions);
            int index = GetBrokenPotionIndex(potions);

            if (potionToRepair == null && index == -1)
            {
                Debug.Log("[Chest] Aucune fiole brise à reparer.");
                return;
            }
            animator.SetTrigger("Open");
            potionToRepair.Repair();
            SaveManager.UnlockPotionSlot(index);
            Sprite emptySprite = potionToRepair.GetSpriteEmpty();
            _isUsed = true;
            fPromptUI.SetActive(false);
            StartCoroutine(AnimateFloatingPotion(emptySprite));
            SoundFXManager.instance.PlaySoundFXClip(openChestSound, transform, 1f);
        }

        private int GetBrokenPotionIndex(List<Scripts.Alcohol> potions)
        {
            for (int i = 0; i < potions.Count; i++)
            {
                if (!SaveManager.IsPotionSlotUnlocked(i))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
