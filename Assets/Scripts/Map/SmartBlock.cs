using UnityEngine;

public class SmartBlock : MonoBehaviour
{
    public string groundLayer = "Ground";
    public string wallLayer = "Wall";

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        foreach (var contact in collision.contacts)
        {
            // Now checking for player on top (normal pointing down)
            if (Vector2.Angle(contact.normal, Vector2.down) < 45f)
            {
                gameObject.layer = LayerMask.NameToLayer(groundLayer);
                return;
            }
        }

        // If no top collision found, default to wall
        gameObject.layer = LayerMask.NameToLayer(wallLayer);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer(wallLayer);
        }
    }
}