using System.Collections;
using System.Collections.Generic;
using Alcohol;
using Managers;
using Map;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class Chest : Interactable
    {
        [SerializeField] private Transform floatingPoint;
        [SerializeField] private GameObject floatingPotionPrefab;
        [SerializeField] private AudioClip openChestSound;
        [SerializeField] private Animator animator;


        private void Start()
        {
            fPromptUI.SetActive(false);
        }

        public override void Interact(PlayerController player)
        {
            if (_isUsed) return;
            

            List<Alcohol> potions = player.GetPotions();

            Alcohol potionToRefill = GetEmptyPotion(potions);

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

        private Alcohol GetEmptyPotion(List<Alcohol> potions)
        {
            List<Alcohol> emptyPotions = new List<Alcohol>();

            foreach (var potion in potions)
            {
                if (potion.state == State.Empty)
                {
                    emptyPotions.Add(potion);
                }
            }

            if (emptyPotions.Count == 0)
                return null;

            int index = Random.Range(0, emptyPotions.Count);
            return emptyPotions[index];
        }

        private IEnumerator AnimateFloatingPotion(Sprite sprite)
        {
            GameObject floating = Instantiate(floatingPotionPrefab, floatingPoint.position, Quaternion.identity);
            SpriteRenderer sr = floating.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;

            Vector3 start = floating.transform.position;
            Vector3 end = start + new Vector3(0f, 1.5f, 0f);
            float duration = 2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                floating.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(floating);
        }
    }
}
