using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Map
{
    public class Room : MonoBehaviour
    {
        [System.Serializable]
        public class Door
        {
            public Direction direction;
            public Transform doorTransform;
            public bool isUsed = false;
            public Room connectedRoom;
        }

        public List<Door> doors = new();
        public List<Transform> spawnPoints = new();
        public int wantedNbSpawn;
        public List<GameObject> ennemies = new();
        public List<GameObject> ennemieToUse = new();
        public Collider2D[] colliders;
        public int depth;

        private GameObject _enemyCounter;
        private Text _enemyCounterText;
        private int _enemyCountNumber;

        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();

            foreach (var door in doors)
            {
                var col = door.doorTransform.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                var sr = door.doorTransform.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
            }
        }

        private void Update()
        {
            if (!_enemyCounterText) return;
            _enemyCountNumber = ennemies.Count;
            _enemyCounterText.text = $"Enemies : {_enemyCountNumber}";
        }


        public void SpawnEnemies()
        {
            List<Transform> availablePoints = new(spawnPoints);

            for (int i = 0; i < wantedNbSpawn && availablePoints.Count > 0; i++)
            {
                int spawnIndex = Random.Range(0, availablePoints.Count);
                Transform spawnPoint = availablePoints[spawnIndex];
                availablePoints.RemoveAt(spawnIndex);

                GameObject prefab = ennemieToUse[Random.Range(0, ennemieToUse.Count)];
                GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity, transform);

                ennemies.Add(enemy);
            }
        }

        public void EnableUnusedDoor()
        {
            foreach (var door in doors)
            {
                if (!door.isUsed)
                {
                    var col = door.doorTransform.GetComponent<Collider2D>();
                    if (col != null) col.enabled = true;

                    var sr = door.doorTransform.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = true;
                }
            }
        }

        public void EnableUsedDoor()
        {
            foreach (var door in doors)
            {
                if (door.isUsed)
                {
                    var col = door.doorTransform.GetComponent<Collider2D>();
                    if (col != null) col.enabled = true;
                    var sr = door.doorTransform.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = true;
                }
            }
        }

        public void DisableUsedDoor()
        {
            foreach (var door in doors)
            {
                if (door.isUsed)
                {
                    var col = door.doorTransform.GetComponent<Collider2D>();
                    if (col != null) col.enabled = false;

                    var sr = door.doorTransform.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false;
                }
            }
        }

        public void OnPlayerEnter()
        {
            _enemyCounter = GameObject.FindWithTag("EnemyCounter");
            if (_enemyCounter != null)
            {
                Debug.Log("EnemyCounter found!");

                _enemyCounterText = _enemyCounter.GetComponent<Text>();
            }

            Debug.Log("EnemyCounter is null");

            if (ennemies.Count != 0)
            {
                EnableUsedDoor();
            }
        }

        public void OnEnemyDied(GameObject enemy)
        {
            ennemies.Remove(enemy);

            if (ennemies.Count == 0)
            {
                DisableUsedDoor();
            }
        }
    }
}