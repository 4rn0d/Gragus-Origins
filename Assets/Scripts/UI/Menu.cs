using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class Menu : MonoBehaviour
    {
        
        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
        
    }
    
}
