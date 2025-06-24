using UnityEngine;

namespace UI
{
    public class Alcohol : MonoBehaviour
    {
        [SerializeField] GameObject alcohol;

        public void changeAlcohol(GameObject newAlcohol)
        {
            alcohol = newAlcohol;
        }
    }
}