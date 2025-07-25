using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] GameObject pauseMenuUI;
        [SerializeField] GameObject settingsMenuUI;
        [SerializeField] GameObject volumeMenuUI;
        
        public bool isPaused = false;

        void Start()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            volumeMenuUI.SetActive(false);
        }
        
        public void TogglePause()
        {
            Debug.Log("TogglePause called");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            pauseMenuUI.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;
        }
        
        private void ResumeGame()
        {
            pauseMenuUI.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }

        public void ToMenu()
        {
            GameObject obj = GameObject.Find("Gragus(Clone)");
            Destroy(obj);
            Debug.Log("Going to Main Menu");
            SceneManager.LoadScene("MainMenu");
            ResumeGame();
        }
        
    }
    
}
