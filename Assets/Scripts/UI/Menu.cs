using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class Menu : MonoBehaviour
    {
        public GameObject thankYouPanel;
        
        private void Start()
        {
            // Check if coming from boss death
            if (PlayerPrefs.GetInt("BossDefeated", 0) == 1)
            {
                if (thankYouPanel != null)
                    thankYouPanel.SetActive(true);

                PlayerPrefs.SetInt("BossDefeated", 0);
            }
            else
            {
                if (thankYouPanel != null)
                    thankYouPanel.SetActive(false);
            }
        }
        
        public void PlayGame()
        {
            Debug.Log(SceneManager.GetActiveScene().buildIndex);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
        
    }
    
}
