using System.Collections;

namespace Enemy
{
    using UnityEngine;

    using UnityEngine;

    public class SlowableEnemy : MonoBehaviour
    {
        [SerializeField] private float originalSpeed = 2f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private float currentSpeed;
        private Coroutine slowRoutine;

        public float CurrentSpeed => currentSpeed;

        private void Awake()
        {
            currentSpeed = originalSpeed;
            
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Slow(float factor, float duration)
        {
            if (slowRoutine != null)
            {
                StopCoroutine(slowRoutine);
            }

            slowRoutine = StartCoroutine(SlowEffect(factor, duration));
        }

        private IEnumerator SlowEffect(float factor, float duration)
        {
            Debug.Log($"[SlowableEnemy] Start slow for {duration} seconds");
            currentSpeed = originalSpeed * factor;

            if (spriteRenderer != null)
                spriteRenderer.color = Color.magenta;

            yield return new WaitForSeconds(duration);

            Debug.Log("[SlowableEnemy] Ending slow");

            currentSpeed = originalSpeed;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;

            slowRoutine = null;
        }
    }

}