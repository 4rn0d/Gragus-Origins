using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class DeathScreen : MonoBehaviour
    {
        
        public void PlayAgain()
        {
            SceneManager.LoadScene(2);
        }
        
        public void QuitGame()
        {
            Debug.Log("Go back to main menu");
            SceneManager.LoadScene(1);
        }
        
    }
    
}