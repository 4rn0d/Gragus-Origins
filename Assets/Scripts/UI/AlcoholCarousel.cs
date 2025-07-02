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

        private Dictionary<Alcohol, Alcohol> _alcoholInstances = new Dictionary<Alcohol, Alcohol>();
        private List<Alcohol> _spawnedAlcohols = new List<Alcohol>();
        private int _currentIndex = 0;

        private void Start()
        {
            foreach (var prefab in alcoholList)
            {
                var instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);
                instance.ChangeState(State.Full); 
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
            if (alcoholList.Count == 0)
                return;

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
                if (posIndex == 0)
                {
                    alcohol.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    alcohol.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                alcohol.gameObject.SetActive(true);
                _spawnedAlcohols.Add(alcohol);
            }
        }

        public Alcohol GetCurrentAlcohol()
        {
            return alcoholList[_currentIndex];
        }

        public Alcohol NextPotion()
        {
            _currentIndex = (_currentIndex + 1) % alcoholList.Count;
            ShowPotions(_currentIndex);
            return _alcoholInstances[alcoholList[_currentIndex]];
        }

    }
}