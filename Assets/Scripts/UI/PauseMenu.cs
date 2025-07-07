using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        private bool _isPaused = false;

        void Start()
        {
            gameObject.SetActive(false);
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
            gameObject.SetActive(true);
            _isPaused = true;
            Time.timeScale = 0f;
        }
        
        private void ResumeGame()
        {
            gameObject.SetActive(false);
            _isPaused = false;
            Time.timeScale = 1f;
        }
    }
}