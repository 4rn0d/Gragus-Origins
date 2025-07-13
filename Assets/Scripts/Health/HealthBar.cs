using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Health.Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<Health.Health>();
        
        // Ensure total bar is full at start
        totalhealthBar.fillAmount = 1f;
    }

    private void Update()
    {
        float targetFill = playerHealth.currentHealth / playerHealth.startingHealth;
        currenthealthBar.fillAmount = Mathf.Lerp(currenthealthBar.fillAmount, targetFill, 10f * Time.deltaTime);
    }

}