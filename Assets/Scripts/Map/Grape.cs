using Managers;
using Scripts;
using UnityEngine;

namespace Map
{
    public class Grape : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                player.addGrape();
                Destroy(gameObject);
            }
        }
    }
}