namespace Map
{
    using UnityEngine;

    public class FloorTransition : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DungeonManager.Instance.GoToNextFloor();
            }
        }
    }
}