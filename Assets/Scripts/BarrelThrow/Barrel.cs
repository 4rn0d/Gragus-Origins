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
    private SpriteRenderer _explosionSprite;

    private void Awake()
    {
        _explosionSprite = explosionEffect.GetComponent<SpriteRenderer>();
        
    }

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

    public void InitalizeBarrel(PlayerController player, float speed)
    {
        //Play Barrel Sound
        SoundFXManager.instance.PlaySoundFXClip(barrelSound, transform, 1f);
        
        if (player.GetComponentInParent<Rigidbody2D>().linearVelocity.magnitude <= 1f)
        {
            rb.linearVelocity = speed * transform.right;
        }
        else
        {
            rb.linearVelocity = (player.GetComponentInParent<Rigidbody2D>().linearVelocity.magnitude + speed) * transform.right;
        }
        _explosionSprite.color = player._alcoholBar.color;
    }
    
    public void StopBarrel()
    {
        _isStopping = true;
    }

    public void ExplodeBarrel(PlayerController player)
    {
        Instantiate(explosionEffect, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.2f), new Quaternion(0, 0, 0, 0));
        Destroy(gameObject);
    }
    
}
