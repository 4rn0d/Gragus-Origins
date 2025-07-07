using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts
{
    
    public class Menu : MonoBehaviour
    {
        
        public void PlayGame()
        {
            SceneManager.LoadScene("POC_ProceduralyGeneratedMap");
        }
        
        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
        
    }
    
}
