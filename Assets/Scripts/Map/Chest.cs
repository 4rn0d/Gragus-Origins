using System.Collections;
using System.Collections.Generic;
using Managers;
using Scripts;
using UnityEngine;

namespace Map
{
    public abstract class Chest : Interactable
    {
        [SerializeField] protected Transform floatingPoint;
        [SerializeField] protected GameObject floatingPotionPrefab;
        [SerializeField] protected AudioClip openChestSound;
        [SerializeField] protected Animator animator;


        protected void Start()
        {
            fPromptUI.SetActive(false);
        }

        public abstract override void Interact(PlayerController player);

        protected Scripts.Alcohol GetEmptyPotion(List<Scripts.Alcohol> potions)
        {
            List<Scripts.Alcohol> emptyPotions = new List<Scripts.Alcohol>();

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
        protected Scripts.Alcohol GetBrokenPotion(List<Scripts.Alcohol> potions)
        {
            foreach (var potion in potions)
            {
                if (potion.state == State.Broken)
                {
                    return potion;
                }
            }
            return null;
        }

        protected IEnumerator AnimateFloatingPotion(Sprite sprite)
        {
            GameObject floating = Instantiate(floatingPotionPrefab, floatingPoint.position, Quaternion.identity);
            SpriteRenderer sr = floating.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;

            Vector3 start = floating.transform.position;
            Vector3 end = start + new Vector3(0f, 0.5f, 0f);
            float duration = 2.5f;
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
