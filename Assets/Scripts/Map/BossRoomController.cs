using Health;
using UnityEngine;

namespace Map
{
    public class BossRoomController : MonoBehaviour
    {
        public GameObject bossPrefab;
        public Transform spawnPoint;
        public GameObject portalPrefab;

        private void Start()
        {
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

            BaseHealth bossHealth = boss.GetComponent<BaseHealth>();
            if (bossHealth != null)
            {
                bossHealth.onDeath.AddListener(OnBossDeath);
            }
            else
            {
                Debug.LogWarning("Boss prefab does not have a BaseHealth component.");
            }
        }

        private void OnBossDeath()
        {
            Debug.Log("Boss died — spawning portal!");
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
        }
    }
}