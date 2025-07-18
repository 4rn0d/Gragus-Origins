using System;
using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(
        name: "Enemy Can See Player",
        story: "[Enemy] has line of sight to [Player] (blocked by Ground or Wall layers)",
        category: "Conditions")]
    public partial class EnemyCanSeePlayerCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] public BlackboardVariable<GameObject> Enemy;

        public override bool IsTrue()
        {
            if (Player == null || Player.Value == null)
            {
                Debug.LogWarning("Player is null");
                return false;
            }

            if (Enemy == null || Enemy.Value == null)
            {
                Debug.LogWarning("Enemy is null");
                return false;
            }

            Vector2 enemyPos = Enemy.Value.transform.position;
            Vector2 playerPos = Player.Value.transform.position;
            Vector2 direction = (playerPos - enemyPos).normalized;
            float distance = Vector2.Distance(enemyPos, playerPos);

            Debug.DrawLine(enemyPos, playerPos, Color.red, 0.1f);

            // Raycast against everything
            RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, distance);

            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;
                
                if (hitLayer == 6 || hitLayer == 7) // 6 = Ground, 7 = Wall, 13 = Map
                {
                    Debug.Log($"[Condition] Line of sight blocked by: {hit.collider.name} on layer {hitLayer}");
                    return false;
                }
            }

            Debug.Log("[Condition] Enemy can see player");
            return true;
        }
    }
}