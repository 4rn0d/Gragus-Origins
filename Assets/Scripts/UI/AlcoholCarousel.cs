using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace Scripts
{
    
    public class AlcoholCarousel : MonoBehaviour
    {
        [Header("Alcohol")]
        [SerializeField] List<Alcohol> alcoholList;
        [SerializeField] List<Transform> positionList;
        
        private List<Alcohol> _spawnedAlcohols = new List<Alcohol>();
        private int _currentIndex = 0;

        private void Start()
        {
            ShowPotions(_currentIndex);
        }

        private void ClearPreviousPotions()
        {
            foreach (var obj in _spawnedAlcohols)
            {
                Destroy(obj.gameObject);
            }
            _spawnedAlcohols.Clear();
        }

        public void ShowPotions(int centerIndex)
        {
            if (alcoholList.Count == 0)
                return;

            ClearPreviousPotions();

            int count = alcoholList.Count;
            for (int i = 0; i <= 4; i++)
            {
                int posIndex = i;
                int alcoholIndex = (centerIndex + i + count) % count;
                Alcohol alcohol = Instantiate(
                    alcoholList[alcoholIndex],
                    positionList[posIndex].position,
                    positionList[posIndex].rotation,
                    positionList[posIndex]
                );
                if (posIndex == 0)
                {
                    alcohol.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                _spawnedAlcohols.Add(alcohol);
            }
        }

        public Alcohol GetCurrentAlcohol()
        {
            Debug.Log(alcoholList[_currentIndex]);
            Debug.Log(_currentIndex);
            return alcoholList[_currentIndex];
        }

        public Alcohol NextPotion()
        {
            _currentIndex = (_currentIndex + 1) % alcoholList.Count;
            ShowPotions(_currentIndex);
            return alcoholList[_currentIndex];
        }

    }
}