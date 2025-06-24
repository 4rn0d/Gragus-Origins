using System.Collections.Generic;
using UnityEngine;

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
            var realColliders = new List<Collider2D>();
            foreach (var col in colliders)
            {
                if (!col.isTrigger) realColliders.Add(col);
            }
            if (realColliders.Count == 0) return new Bounds(transform.position, Vector3.zero);

            Bounds bounds = realColliders[0].bounds;
            for (int i = 1; i < realColliders.Count; i++)
            {
                bounds.Encapsulate(realColliders[i].bounds);
            }
            return bounds;

        }

        public Door GetUnusedDoor()
        {
            foreach (var door in doors)
                if (!door.isUsed) return door;
            return null;
        }
        
        public Collider2D[] colliders;
        void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

    }
}