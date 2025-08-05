using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Scripts
{
    public class DeathScreen : MonoBehaviour
    {
        [SerializeField] private GameObject firstSelected;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null); // Clear any previous selection
            EventSystem.current.SetSelectedGameObject(firstSelected); // Set new selection
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene("ProceduralyGeneratedMap");
        }

        public void QuitGame()
        {
            Debug.Log("Go back to main menu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}