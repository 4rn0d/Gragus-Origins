using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    [SerializeField] private float damageToPlayer = 25f;
    [SerializeField] private float damageToEnemy = 25f;
    [SerializeField] private AudioClip dashDamageSound;
    [SerializeField] private AudioClip meleeHitSound;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var controller = other.GetComponent<Scripts.PlayerController>();
        var playerHealth = other.GetComponent<Health.PlayerHealth>();

        if (controller != null && playerHealth != null)
        {
            if (controller.IsThePlayerDashing())
            {
                GetComponent<MeleeDamage>().enabled = false;
                StartCoroutine(ReenableDamage());
                
                var enemyHealth = GetComponent<Health.EnemyHealth>();
                if (enemyHealth != null)
                {
                    SoundFXManager.instance.PlaySoundFXClip(dashDamageSound, transform, 1f);
                    enemyHealth.TakeDamage(damageToEnemy);
                    Debug.Log("Player dashed into enemy — enemy took damage.");
                }
            }
            else
            {
                if (!playerHealth.invulnerable)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    SoundFXManager.instance.PlaySoundFXClip(meleeHitSound, transform, 1f);
                    Debug.Log("Enemy hit player — player took damage.");
                }
                else
                {
                    Debug.Log("Player is invulnerable — no damage taken.");
                }
            }
        }
    }
    
    private IEnumerator ReenableDamage()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<MeleeDamage>().enabled = true;
    }

}