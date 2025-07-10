using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Enemy
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Follow Player 2D", story: "Moves the enemy towards the player if in range.", category: "Action", id: "4227d1b9729241013dcce6ee6b15f460")]
    public partial class FollowPlayer2DAction : Action
    {

        [SerializeReference] public BlackboardVariable<GameObject> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] public BlackboardVariable<float> Speed = new(3f);
        [SerializeReference] public BlackboardVariable<float> StopRange = new(0.5f); // How close before stopping
        [SerializeReference] public BlackboardVariable<float> MaxChaseRange = new(10f);

        
        private Transform _enemyTransform;
        private Vector3 _initScale;

        protected override Status OnStart()
        {
            if (Enemy?.Value == null || Player?.Value == null)
                return Status.Failure;

            _enemyTransform = Enemy.Value.transform;
            _initScale = _enemyTransform.localScale;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_enemyTransform == null || Player?.Value == null)
                return Status.Failure;

            Vector2 playerPos = Player.Value.transform.position;
            Vector2 enemyPos = _enemyTransform.position;

            float distance = Vector2.Distance(playerPos, enemyPos);

            // Stop chasing if player is out of range
            if (distance > MaxChaseRange.Value) // <-- adjust this multiplier if needed
            {
                Debug.Log("[Follow2D] Player out of chase range — stop chasing.");
                return Status.Failure;
            }

            // Stop if already very close to the player
            if (distance <= StopRange.Value)
            {
                Debug.Log("[Follow2D] Reached player — stop moving.");
                return Status.Success;
            }

            // Continue following
            Vector2 direction = (playerPos - enemyPos).normalized;
            _enemyTransform.position += (Vector3)(direction * Speed.Value * Time.deltaTime);

            // Flip sprite
            if (direction.x != 0)
            {
                _enemyTransform.localScale = new Vector3(
                    Mathf.Sign(direction.x) * Mathf.Abs(_initScale.x),
                    _initScale.y,
                    _initScale.z
                );
            }

            return Status.Running;
        }


        protected override void OnEnd()
        {
        }
    }
}

