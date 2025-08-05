using System.Collections.Generic;
using Player;
using Scripts;
using UnityEngine;

namespace UI
{
    public class AlcoholCarousel : MonoBehaviour
    {
        [Header("Alcohol")]
        [SerializeField] List<Scripts.Alcohol> alcoholList;        // Prefabs
        [SerializeField] List<Transform> positionList;     // UI Positions

        private Dictionary<int, Scripts.Alcohol> _alcoholInstances = new();  // Use index as key
        private List<Scripts.Alcohol> _spawnedAlcohols = new();
        private int _currentIndex = 0;

        private void Awake()
        {
            // Fallback loading if list is empty (in case it's stripped)
            if (alcoholList == null || alcoholList.Count == 0 || alcoholList[0] == null)
            {
                alcoholList = new List<Scripts.Alcohol>(Resources.LoadAll<Scripts.Alcohol>("Alcohols"));
                Debug.LogWarning("[AlcoholCarousel] alcoholList was empty. Loaded from Resources.");
            }

            for (int i = 0; i < alcoholList.Count; i++)
            {
                Scripts.Alcohol prefab = alcoholList[i];
                if (prefab == null)
                {
                    Debug.LogError($"[AlcoholCarousel] alcoholList[{i}] is null!");
                    continue;
                }

                Scripts.Alcohol instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);

                if (SaveManager.IsPotionSlotUnlocked(i))
                {
                    instance.ChangeState(i == 0 ? State.Full : State.Empty);
                }
                else
                {
                    instance.ChangeState(State.Broken);
                }

                _alcoholInstances[i] = instance;
            }

            ShowPotions(_currentIndex);
        }



        private void ClearPreviousPotions()
        {
            foreach (var alcohol in _spawnedAlcohols)
            {
                alcohol.gameObject.SetActive(false);
            }
            _spawnedAlcohols.Clear();
        }

        public void ShowPotions(int centerIndex)
        {
            if (alcoholList.Count == 0) return;

            ClearPreviousPotions();
            int count = alcoholList.Count;

            for (int i = 0; i <= 4; i++)
            {
                int posIndex = i;
                int alcoholIndex = (centerIndex + i + count) % count;

                if (!_alcoholInstances.ContainsKey(alcoholIndex))
                {
                    Debug.LogWarning($"[AlcoholCarousel] Missing alcohol instance for index {alcoholIndex}");
                    continue;
                }

                Scripts.Alcohol alcohol = _alcoholInstances[alcoholIndex];
                alcohol.transform.SetParent(positionList[posIndex]);
                alcohol.transform.position = positionList[posIndex].position;
                alcohol.transform.rotation = positionList[posIndex].rotation;
                alcohol.transform.localScale = (posIndex == 0) ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.2f, 0.2f, 0.2f);

                alcohol.gameObject.SetActive(true);
                _spawnedAlcohols.Add(alcohol);
            }
        }

        public Scripts.Alcohol GetCurrentAlcohol()
        {
            if (!_alcoholInstances.ContainsKey(_currentIndex))
            {
                Debug.LogError($"[AlcoholCarousel] Alcohol index '{_currentIndex}' not found.");
                return null;
            }
            return _alcoholInstances[_currentIndex];
        }

        public Scripts.Alcohol NextPotion()
        {
            _currentIndex = (_currentIndex + 1) % alcoholList.Count;
            ShowPotions(_currentIndex);
            return GetCurrentAlcohol();
        }

        public List<Scripts.Alcohol> GetAllAlcohols()
        {
            return _spawnedAlcohols;
        }
    }
}
