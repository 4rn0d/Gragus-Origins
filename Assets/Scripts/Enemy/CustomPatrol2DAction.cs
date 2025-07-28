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
            description: "Moves a GameObject back and forth between waypoints using simple 2D transform movement. Returns Failure if Player is in range.",
            category: "Action/Navigation")]
        public class CustomPatrol2DAction : Action
        {
            [SerializeReference] public BlackboardVariable<GameObject> Agent;
            [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
            [SerializeReference] public BlackboardVariable<GameObject> Player;
            [SerializeReference] public BlackboardVariable<float> PlayerRange = new(3f);
            [SerializeReference] private SlowableEnemy slowable;
            [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new(1.0f);
            [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
            [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);

            private LedgeDetector _ledgeDetector;
            private Rigidbody2D _rb;
            private Transform _agent;
            private Vector3 _initScale;
            private int _currentPoint = 0;
            private float _waitTimer = 0f;
            private bool _waiting = false;

            protected override Status OnStart()
            {
                if (Agent?.Value == null || Waypoints?.Value == null || Waypoints.Value.Count == 0 || Player?.Value == null)
                    return Status.Failure;

                _agent = Agent.Value.transform;
                _initScale = _agent.localScale;

                _ledgeDetector = Agent.Value.GetComponent<LedgeDetector>();
                _rb = Agent.Value.GetComponent<Rigidbody2D>();
                slowable = Agent.Value.GetComponent<SlowableEnemy>();

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
            
                float playerDistance = Vector2.Distance(_agent.position, Player.Value.transform.position);
                if (playerDistance <= PlayerRange.Value && HasLineOfSightToPlayer(_agent.position, Player.Value.transform.position))
                {
                    Debug.Log("[Patrol2D] Player in range and visible — stop patrolling.");
                    if (_rb != null)
                        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);

                    return Status.Failure;
                }
            
                Vector2 currentTarget = new Vector2(Waypoints.Value[_currentPoint].transform.position.x, _agent.position.y);
                Vector2 agentPos = _agent.position;
                float distance = Vector2.Distance(agentPos, currentTarget);
                float dir = Mathf.Sign(currentTarget.x - agentPos.x);
                float moveSpeed = slowable != null ? slowable.CurrentSpeed : 2f;
            
                if (_ledgeDetector != null)
                {
                    bool shouldTurn;
                    if (_ledgeDetector.ShouldJumpOrTurn(dir, out shouldTurn))
                    {
                        Debug.Log("[Patrol2D] Jumping over obstacle.");
                        _ledgeDetector.Jump();
                    }
                    else if (shouldTurn)
                    {
                        Debug.Log("[Patrol2D] Wall too tall — turning back.");
                        AdvanceToNextWaypoint();
                        return Status.Running;
                    }
                    else if (_ledgeDetector.IsLedgeAhead(dir))
                    {
                        if (_ledgeDetector.CanDropFromLedge(dir))
                        {
                            Debug.Log("[Patrol2D] Dropping from ledge.");
                        }
                        else
                        {
                            Debug.Log("[Patrol2D] Ledge ahead — turning back.");
                            AdvanceToNextWaypoint();
                            return Status.Running;
                        }
                    }
                }
            
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
                    Vector2 moveDir = new Vector2(dir, 0);
                    Vector2 velocity = _rb.linearVelocity;
                    velocity.x = moveDir.x * moveSpeed;
                    _rb.linearVelocity = velocity;

                    if (dir != 0)
                    {
                        _agent.localScale = new Vector3(dir * Mathf.Abs(_initScale.x), _initScale.y, _initScale.z);
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

            private bool HasLineOfSightToPlayer(Vector2 from, Vector2 to)
            {
                Vector2 direction = (to - from).normalized;
                float distance = Vector2.Distance(from, to);
                int mask = (1 << 6) | (1 << 7) | (1 << 13);

                RaycastHit2D hit = Physics2D.Raycast(from, direction, distance, mask);
                if (hit.collider != null)
                {
                    Debug.Log($"[Patrol2D] LOS blocked by {hit.collider.name}");
                    return false;
                }

                Debug.DrawLine(from, to, Color.green);
                return true;
            }
        }
    }
