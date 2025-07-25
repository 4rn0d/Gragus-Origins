using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class Menu : MonoBehaviour
    {
        public GameObject thankYouPanel;
        
        private void Start()
        {
            if (PlayerPrefs.GetInt("BossDefeated") == 1)
            {
                if (thankYouPanel != null)
                    thankYouPanel.SetActive(true);

                PlayerPrefs.SetInt("BossDefeated", 0);
            }
            else
            {
                Debug.Log("test");
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

        public void ResetProgress()
        {
            SaveManager.ResetPotionSlots();
        }
        
    }
    
}
