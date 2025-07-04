using UnityEngine;

namespace Enemy
{
    public class RangedEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float shootInterval = 2f;
        [SerializeField] private GameObject player;

        private float shootTimer;

        void Update()
        {
            if (player == null) return;

            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }

        void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            Vector2 directionToPlayer = (player.transform.position - firePoint.position).normalized;

            // Set the bullet's movement direction
            projectile.GetComponent<Projectile>().SetDirection(directionToPlayer);

            // Rotate the dart so the tip faces the direction it's going
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }


    }
}