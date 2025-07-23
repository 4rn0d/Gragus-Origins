using UnityEngine.SceneManagement;

namespace Map
{
    using UnityEngine;

    public class FinalFloorTransition : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene("MainMenu");
                Destroy(other.gameObject);
            }
        }
    }
}