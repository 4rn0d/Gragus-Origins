using Scripts;
using UnityEngine;

namespace Map
{
    public abstract class Interactable : MonoBehaviour
    {
        public bool _isUsed;
        [SerializeField] protected GameObject fPromptUI;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isUsed && other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetNearbyInteractable(this);
                    fPromptUI.SetActive(true);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.ClearNearbyInteractable(this);
                    fPromptUI.SetActive(false);
                }
            }
        }
        public abstract void Interact(PlayerController player);
    }
}