using System;
using UnityEngine;

public class TeleportBack : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.CompareTag("Player"))
      {
         Debug.Log("Teleport back");
         other.transform.position = new Vector3(-10f, -4f, 0f);
      }
   }
}
