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
                Destroy(other.gameObject);
                SceneManager.LoadScene("POC_FinalBoss");
            }
        }
    }
}