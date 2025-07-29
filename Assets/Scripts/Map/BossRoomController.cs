using System;
using Health;
using Scripts;
using UnityEngine;

namespace Map
{
    public class BossRoomController : MonoBehaviour
    {
        public GameObject bossPrefab;
        public Transform bossSpawnPoint;
        public Transform playerSpawnPoint;
        public GameObject portalPrefab;
        public GameObject chestPrefab;
        public GameObject playerPrefab;
        private BaseHealth _health;
        private bool done = false;
        private void Start()
        {
            chestPrefab.SetActive(false);
            GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            bossPrefab = boss;
            _health = boss.GetComponentInChildren<EnemyHealth>();
            GameObject obj = GameObject.Find("Gragus(Clone)");
            
            if (obj == null)
            {
                obj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            }
            PlayerController controller = obj.GetComponent<PlayerController>();
            controller.gameObject.transform.position = playerSpawnPoint.position;
        }

        private void Update()
        {
            if (_health.dead && !done)
            {
                OnBossDeath();
            }
        }

        private void OnBossDeath()
        {
            Instantiate(portalPrefab, bossSpawnPoint.position, Quaternion.identity);
            PlayerPrefs.SetInt("BossDefeated", 1);
            chestPrefab.SetActive(true);
            done = true;
        }
    }
}