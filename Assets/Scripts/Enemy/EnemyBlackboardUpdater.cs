using UnityEngine;
using Unity.Behavior;

namespace Enemy
{
    public class EnemyBlackboardUpdater : MonoBehaviour
    {
        public GameObject player; // Drag the Player GameObject here

        private BehaviorGraphAgent agent;

        void Start()
        {
            agent = GetComponent<BehaviorGraphAgent>();

            // Optional: Set Enemy and Player in blackboard if needed by conditions
            if (agent != null)
            {
                agent.SetVariableValue("Enemy", gameObject);
                agent.SetVariableValue("Player", player);
            }
        }

        void Update()
        {
            if (agent != null && player != null)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);
                agent.SetVariableValue("DistanceToPlayer", distance); // Store in blackboard
                // Optional for debug:
                //Debug.Log($"[Updater] Distance to player: {distance}");
            }
        }
    }
}