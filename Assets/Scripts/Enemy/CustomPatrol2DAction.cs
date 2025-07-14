using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Enemy
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [NodeDescription(
        name: "Patrol 2D",
        description:
        "Moves a GameObject back and forth between waypoints using simple 2D transform movement. Returns Failure if Player is in range.",
        category: "Action/Navigation")]
    public class CustomPatrol2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
        [SerializeReference] public BlackboardVariable<GameObject> Player;
        [SerializeReference] public BlackboardVariable<float> PlayerRange = new(3f);
        [SerializeReference] public BlackboardVariable<float> Speed = new(2f);
        [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
        [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);

        private Transform _agent;
        private Vector3 _initScale;
        private int _currentPoint = 0;
        private float _waitTimer = 0f;
        private bool _waiting = false;

        protected override Node.Status OnStart()
        {
            if (Agent?.Value == null || Waypoints?.Value == null || Waypoints.Value.Count == 0 || Player?.Value == null)
            {
                return Status.Failure;
            }

            _agent = Agent.Value.transform;
            _initScale = _agent.localScale;

            if (!PreserveLatestPatrolPoint.Value)
                _currentPoint = 0;

            _waitTimer = 0f;
            _waiting = false;

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_agent == null || Waypoints?.Value == null || Waypoints.Value.Count == 0 || Player?.Value == null)
                return Status.Failure;

            // Check if Player is in range
            float playerDistance = Vector2.Distance(_agent.position, Player.Value.transform.position);
            if (playerDistance <= PlayerRange.Value)
            {
                Debug.Log("[Patrol2D] Player in range — stop patrolling.");
                return Status.Failure;
            }

            Vector2 currentTarget = Waypoints.Value[_currentPoint].transform.position;
            Vector2 agentPos = _agent.position;
            float distance = Vector2.Distance(agentPos, currentTarget);

            if (_waiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _waiting = false;
                    AdvanceToNextWaypoint();
                }
            }
            else if (distance <= DistanceThreshold.Value)
            {
                _waiting = true;
                _waitTimer = WaypointWaitTime.Value;
            }
            else
            {
                Vector2 direction = (currentTarget - agentPos).normalized;
                _agent.position += (Vector3)(direction * Speed.Value * Time.deltaTime);

                // Flip sprite based on direction
                if (direction.x != 0)
                {
                    _agent.localScale = new Vector3(
                        Mathf.Sign(direction.x) * Mathf.Abs(_initScale.x),
                        _initScale.y,
                        _initScale.z
                    );
                }
            }

            return Status.Running;
        }

        private void AdvanceToNextWaypoint()
        {
            _currentPoint = (_currentPoint + 1) % Waypoints.Value.Count;
        }

        protected override void OnEnd()
        {
            _waitTimer = 0f;
            _waiting = false;
        }
    }
}