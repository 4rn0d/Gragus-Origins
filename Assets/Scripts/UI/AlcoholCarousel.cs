using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    
    public class AlcoholCarousel : MonoBehaviour
    {
        [Header("Alcohol")]
        [SerializeField] List<GameObject> alcoholList;
        [SerializeField] Transform alcoholPos1;
        [SerializeField] Transform alcoholPos2;
        [SerializeField] Transform alcoholPos3;
        [SerializeField] Transform alcoholPos4;
        [SerializeField] Transform alcoholPos5;
        private int _currentIndex = 0;

        private GameObject _currentAlcohol;
        
        void Start()
        {
            ShowPotion(_currentIndex);
        }

        public void ShowPotion(int index)
        {
            if (_currentAlcohol != null)
                Destroy(_currentAlcohol);

            if (alcoholList.Count == 0 || index < 0 || index >= alcoholList.Count)
                return;

            _currentAlcohol = Instantiate(alcoholList[index], alcoholPos1);
        }

        public void NextPotion()
        {
            _currentIndex = (_currentIndex + 1) % alcoholList.Count;
            ShowPotion(_currentIndex);
        }

        public void PreviousPotion()
        {
            _currentIndex = (_currentIndex - 1 + alcoholList.Count) % alcoholList.Count;
            ShowPotion(_currentIndex);
        }

    }
}