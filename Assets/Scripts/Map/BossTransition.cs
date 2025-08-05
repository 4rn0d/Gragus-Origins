using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    public class BossTransition : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene("FinalBoss");
                Destroy(other.gameObject);
            }
        }
    }
}