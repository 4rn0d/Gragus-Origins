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
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(transform.position, Vector3.zero);

            Bounds b = renderers[0].bounds;
            foreach (var r in renderers)
                b.Encapsulate(r.bounds);
            return b;
        }

        public Door GetUnusedDoor()
        {
            foreach (var door in doors)
                if (!door.isUsed) return door;
            return null;
        }
    }
}