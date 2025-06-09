using Unity.Cinemachine;
using UnityEngine;

public class NextRoom : MonoBehaviour
{ 
    
    public CinemachineCamera currentCam;
    public CinemachineCamera nextCam;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentCam.Priority = 0;
            nextCam.Priority = 10;  // Higher value activates this one
            Debug.Log("Switched to next camera");

            (currentCam, nextCam) = (nextCam, currentCam);
        }
    }
    
    
}
