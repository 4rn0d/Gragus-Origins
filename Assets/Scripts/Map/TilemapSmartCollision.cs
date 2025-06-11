using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class TilemapSmartCollision : MonoBehaviour
    {
        public Tilemap tilemap;
        public string groundLayer = "Ground";
        public string wallLayer = "Wall";

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            bool touchedWall = false;
            bool touchedGround = false;

            foreach (var contact in collision.contacts)
            {
                float angleDown = Vector2.Angle(contact.normal, Vector2.down);

                if (angleDown < 45f)
                {
                    touchedGround = true;
                }
                else
                {
                    touchedWall = true;
                }
            }

            if (touchedGround)
            {
                gameObject.layer = LayerMask.NameToLayer(groundLayer);
            }
            else if (touchedWall)
            {
                gameObject.layer = LayerMask.NameToLayer(wallLayer);
            }
        }


        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                gameObject.layer = LayerMask.NameToLayer(wallLayer);
            }
        }
    }
}