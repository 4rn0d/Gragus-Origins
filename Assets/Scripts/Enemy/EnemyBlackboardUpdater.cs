using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    public class EnemyBlackboardUpdater : MonoBehaviour
    {
        public GameObject player;
        private BehaviorGraphAgent _agent;

        void Start()
        {
            _agent = GetComponent<BehaviorGraphAgent>();
            if (_agent == null)
            {
                Debug.LogError("BehaviorGraphAgent not found on enemy!");
            }
            else
            {
                Debug.Log("[Updater] Found BehaviorGraphAgent");
            }
        }


        void Update()
        {
            if (player != null && _agent != null)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);
                Debug.Log($"[Updater] Distance to player: {distance}");
                _agent.SetVariableValue("DistanceToPlayer", distance);
            }
        }

    }
}