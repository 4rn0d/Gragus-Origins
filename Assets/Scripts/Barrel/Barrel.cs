using UnityEngine;
using UnityEngine.InputSystem;

public class Barrel : MonoBehaviour
{
    
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] Transform launchOffset;
    
    [SerializeField] float speed = 4.5f;

    public void BarrelThrow(InputAction.CallbackContext context)
    {
        Instantiate(barrelPrefab, launchOffset.position, launchOffset.rotation);
    }
}
