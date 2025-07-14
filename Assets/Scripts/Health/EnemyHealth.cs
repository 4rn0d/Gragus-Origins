using UnityEngine;
using Map;
using Scripts;

namespace Health
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField] private Behaviour[] componentsToDisable;


        public override void TakeDamage(float damage,  PlayerController player)
        {
            Debug.Log("Enemy is taking damage");
            base.TakeDamage(damage, player);
        }

        protected override void Die()
        {
            if (dead) return;

            dead = true;
            Debug.Log("Enemy died.");

            foreach (var comp in componentsToDisable)
                comp.enabled = false;

            // Notify parent room if needed
            Room room = GetComponentInParent<Room>();
            if (room != null)
            {
                GameObject enemyRoot = transform.parent != null ? transform.parent.gameObject : gameObject;
                room.OnEnemyDied(enemyRoot);
                Destroy(enemyRoot);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}