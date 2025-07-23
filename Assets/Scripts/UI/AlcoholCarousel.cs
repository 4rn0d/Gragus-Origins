using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Scripts
{
    public class AlcoholCarousel : MonoBehaviour
    {
        [Header("Alcohol")]
        [SerializeField] List<Alcohol> alcoholList;        // Prefabs
        [SerializeField] List<Transform> positionList;     // UI Positions

        private Dictionary<Alcohol, Alcohol> _alcoholInstances = new();
        private List<Alcohol> _spawnedAlcohols = new();
        private int _currentIndex = 0;

        private void Start()
        {
            for (int i = 0; i < alcoholList.Count; i++)
            {
                Alcohol prefab = alcoholList[i];
                Alcohol instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);

                if (SaveManager.IsPotionSlotUnlocked(i))
                {
                    if (i == 0) instance.ChangeState(State.Full);
                    else instance.ChangeState(State.Empty);
                }
                else
                {
                    instance.ChangeState(State.Broken);
                }

                _alcoholInstances[prefab] = instance;
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

                Alcohol alcohol = _alcoholInstances[alcoholList[alcoholIndex]];
                alcohol.transform.SetParent(positionList[posIndex]);
                alcohol.transform.position = positionList[posIndex].position;
                alcohol.transform.rotation = positionList[posIndex].rotation;
                alcohol.transform.localScale = (posIndex == 0) ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.2f, 0.2f, 0.2f);

                alcohol.gameObject.SetActive(true);
                _spawnedAlcohols.Add(alcohol);
            }
        }

        public Alcohol GetCurrentAlcohol()
        {
            var key = alcoholList[_currentIndex];
            if (!_alcoholInstances.ContainsKey(key))
            {
                Debug.LogError($"[AlcoholCarousel] Alcohol key '{key}' not found.");
                return null;
            }
            return _alcoholInstances[key];
        }

        public Alcohol NextPotion()
        {
            _currentIndex = (_currentIndex + 1) % alcoholList.Count;
            ShowPotions(_currentIndex);
            return _alcoholInstances[alcoholList[_currentIndex]];
        }

        public List<Alcohol> GetAllAlcohols()
        {
            return _spawnedAlcohols;
        }
    }
}
