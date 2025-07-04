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
        
        public Collider2D[] colliders;
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

        public void EnableUnusedDoorVisuals()
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


    }
}