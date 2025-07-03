using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        }

        public List<Door> doors = new();
    
        public Bounds GetBounds()
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            if (colliders.Length == 0) return new Bounds(transform.position, Vector3.zero);

            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }
            return bounds;
        }

        public Door GetUnusedDoor()
        {
            foreach (var door in doors)
                if (!door.isUsed) return door;
            return null;
        }
    }
}