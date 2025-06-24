namespace Map
{
    using UnityEngine;

    public class RoomBoundsVisualizer : MonoBehaviour
    {
        public Color boundsColor = Color.red;

        void OnDrawGizmos()
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            if (colliders.Length == 0) return;

            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }

            Gizmos.color = boundsColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }

}