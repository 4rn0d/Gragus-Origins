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

            _health = boss.GetComponent<BaseHealth>();
        }

        private void Update()
        {
            if (_health && _health.dead && !done)
            {
                OnBossDeath();
            }
        }

        private void OnBossDeath()
        {
            Debug.Log("Boss died — spawning portal!");
            PlayerPrefs.SetInt("BossDefeated", 1);
            //ADD CHEST
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
            done = true;
        }
    }
}