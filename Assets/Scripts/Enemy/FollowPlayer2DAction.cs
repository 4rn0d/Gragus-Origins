using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Enemy
{
    [Serializable]
    public class FollowPlayer2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Enemy;
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] private SlowableEnemy slowable;
        [SerializeReference] public BlackboardVariable<float> StopRange = new(0.5f);
        [SerializeReference] public BlackboardVariable<float> MaxChaseRange = new(10f);

        private Transform _enemyTransform;
        private Vector3 _initScale;
        private Rigidbody2D _rb;
        private LedgeDetector _ledgeDetector;

        protected override Status OnStart()
        {
            if (Enemy?.Value == null || Player?.Value == null) return Status.Failure;

            _enemyTransform = Enemy.Value.transform;
            _initScale = _enemyTransform.localScale;
            _rb = Enemy.Value.GetComponent<Rigidbody2D>();
            slowable = Enemy.Value.GetComponent<SlowableEnemy>();
            _ledgeDetector = Enemy.Value.GetComponent<LedgeDetector>();

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_enemyTransform == null || Player?.Value == null) return Status.Failure;

            Vector2 playerPos = Player.Value.transform.position;
            Vector2 enemyPos = _enemyTransform.position;
            float distance = Vector2.Distance(playerPos, enemyPos);

            if (distance > MaxChaseRange.Value)
            {
                Debug.Log("[Follow2D] Player out of chase range.");
                return Status.Failure;
            }

            if (distance <= StopRange.Value)
            {
                Debug.Log("[Follow2D] Reached player.");
                return Status.Success;
            }

            Vector2 direction = (playerPos - enemyPos).normalized;
            float dir = Mathf.Sign(direction.x);
            float moveSpeed = slowable != null ? slowable.CurrentSpeed : 3f;

            if (_ledgeDetector != null)
            {
                if (_ledgeDetector.IsWallAhead(dir))
                {
                    if (_ledgeDetector.CanJumpOverObstacle(dir))
                    {
                        Debug.Log("[Follow2D] Jumping over wall.");
                        _ledgeDetector.Jump();
                    }
                    else
                    {
                        Debug.Log("[Follow2D] Wall ahead and can’t jump — stopping.");
                        return Status.Failure;
                    }
                }

                if (_ledgeDetector.IsLedgeAhead(dir))
                {
                    Debug.Log("[Follow2D] Ledge ahead — stopping.");
                    return Status.Failure;
                }
            }

            _enemyTransform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            if (direction.x != 0)
            {
                _enemyTransform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(_initScale.x), _initScale.y, _initScale.z);
            }

            return Status.Running;
        }
    }
}
