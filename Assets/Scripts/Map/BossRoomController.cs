using System;
using Health;
using UnityEngine;

namespace Map
{
    public class BossRoomController : MonoBehaviour
    {
        public GameObject bossPrefab;
        public Transform spawnPoint;
        public GameObject portalPrefab;
        public GameObject chestPrefab;
        private BaseHealth _health;
        private bool done = false;
        private void Start()
        {
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            bossPrefab = boss;
            _health = boss.GetComponentInChildren<EnemyHealth>();
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
            Instantiate(portalPrefab, spawnPoint.position, Quaternion.identity);
            PlayerPrefs.SetInt("BossDefeated", 1);
            //ADD CHEST
            done = true;
        }
    }
}