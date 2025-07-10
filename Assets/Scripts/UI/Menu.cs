using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class Menu : MonoBehaviour
    {
        
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
