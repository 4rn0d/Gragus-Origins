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
        
        private bool _isPaused = false;

        void Start()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
        }
        
        public void TogglePause()
        {
            Debug.Log("TogglePause called");
            if (_isPaused)
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
            _isPaused = true;
            Time.timeScale = 0f;
        }
        
        private void ResumeGame()
        {
            pauseMenuUI.SetActive(false);
            _isPaused = false;
            Time.timeScale = 1f;
        }

        public void ToMenu()
        {
            Debug.Log("Going to Main Menu");
            SceneManager.LoadScene(0);
        }
        
    }
    
}
