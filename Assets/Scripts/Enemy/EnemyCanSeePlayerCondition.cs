using System;
using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(
        name: "Enemy Can See Player",
        story: "[Enemy] has line of sight to [Player] (blocked by Ground or Wall layers) [CanSeePlayer]",
        category: "Conditions")]
    public partial class EnemyCanSeePlayerCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] public BlackboardVariable<GameObject> Enemy;
        [SerializeReference] public BlackboardVariable<bool> CanSeePlayer;


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

            int mask = (1 << 6) | (1 << 7) | (1 << 13); // Ground and Wall and Map layers
            RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, distance, mask);

            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;
                string layerName = LayerMask.LayerToName(hitLayer);

                Debug.Log($"[Raycast] Hit {hit.collider.name} on layer {hitLayer} ({layerName})");

                if (hitLayer == 6 || hitLayer == 7 || hitLayer == 13)
                {
                    CanSeePlayer.Value = false;
                    Debug.Log("[Condition] Line of sight BLOCKED var value : " + CanSeePlayer.Value);
                    return false;
                }
            }
            else
            {
                Debug.Log("[Raycast] No collider hit at all");
            }

            CanSeePlayer.Value = true;
            Debug.Log("[Condition] Enemy can see player var value : " +  CanSeePlayer.Value);
            
            return true;
        }
    }
}