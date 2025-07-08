using System;
using UnityEngine;
using Scripts;

public class FountainRefill : MonoBehaviour
{
    [SerializeField] private float refillAmount = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.RefillAlcohol(refillAmount);
            }
        }
    }
}