using UnityEngine;
using UnityEngine.InputSystem;

public class Throw : MonoBehaviour
{
    
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] Transform launchOffset;
    
    public void BarrelThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Instantiate(barrelPrefab, launchOffset.position, launchOffset.rotation);
        }
    }
}
