using Unity.Behavior;
using UnityEngine;

namespace Enemy
{
    [NodeDescription(
        name: "Follow Player 2D",
        description: "Moves the Agent toward the Target using Rigidbody2D.",
        category: "Action/Navigation")]
    public class FollowPlayer2DAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed = new(2f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);

        private Rigidbody2D _rb;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            _rb = Agent.Value.GetComponent<Rigidbody2D>();
            return _rb != null ? Status.Running : Status.Failure;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null || _rb == null)
                return Status.Failure;

            Vector2 agentPos = Agent.Value.transform.position;
            Vector2 targetPos = Target.Value.transform.position;

            float distance = Vector2.Distance(agentPos, targetPos);

            if (distance <= DistanceThreshold.Value)
            {
                _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                return Status.Success;
            }

            Vector2 direction = (targetPos - agentPos).normalized;
            _rb.linearVelocity = new Vector2(direction.x * Speed.Value, _rb.linearVelocity.y);

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_rb != null)
            {
                _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            }
        }
    }
}