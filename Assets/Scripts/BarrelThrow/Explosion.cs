using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float _delay = 1f;
    private bool _canKnockback = true;
    
    
    private void Start()
    {
        Destroy(gameObject, 1f);
    }
    
    private void Update()
    {
        _delay -= Time.deltaTime;
        if (_delay < 0.97f)
        {
            _canKnockback = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (_canKnockback)
            {
                player.BarrelJump();
            }
        }
    }
    
}
