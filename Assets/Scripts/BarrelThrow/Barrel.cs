using System;
using Managers;
using Scripts;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float deceleration = 2f;
    [SerializeField] private AudioClip barrelSound;
    
    private bool _isStopping = false;

    private void FixedUpdate()
    {
        if (_isStopping)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * deceleration);

            if (rb.linearVelocity.magnitude < 0.05f)
            {
                rb.linearVelocity = Vector2.zero;
                _isStopping = false;
            }
        }
    }

    public void InitalizeBarrel(Rigidbody2D player, float speed)
    {
        //Play Barrel Sound
        SoundFXManager.instance.PlaySoundFXClip(barrelSound, transform, 1f);
        
        if (player.linearVelocity.magnitude <= 1f)
        {
            rb.linearVelocity = speed * transform.right;
        }
        else
        {
            rb.linearVelocity = (player.linearVelocity.magnitude + speed) * transform.right;
        }
    }
    
    public void StopBarrel()
    {
        _isStopping = true;
    }

    public void ExplodeBarrel(PlayerController player)
    {
        //Explosion Sound is in the ExplosionScript
        explosionEffect.GetComponent<SpriteRenderer>().color = player._alcoholBar.color;
        Instantiate(explosionEffect, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.2f), new Quaternion(0, 0, 0, 0));
        Destroy(gameObject);
    }
    
}
