using System;
using Managers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to point 2D", story: "[Object] moves to [EndPoint]", category: "Action", id: "b47b73f7f25eb38df598ed0877a0c8e6")]
public partial class MoveToPoint2D : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<GameObject> EndPoint; // Target position
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float reachThreshold = 0.2f;

    private Animator _animator;
    private Transform _objectTransform;
    private Transform _endPointTransform;
    private bool _hasReachedCenter = false;

    protected override Status OnStart()
    {
        if (Object?.Value == null)
        {
            Debug.LogWarning("[Object] Missing references.");
            return Status.Failure;
        }

        _objectTransform = Object.Value.transform;
        _endPointTransform = EndPoint.Value.transform;
        _hasReachedCenter = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_hasReachedCenter)
        {
            float distance = Vector2.Distance(_objectTransform.position, _endPointTransform.position);
            if (distance <= reachThreshold)
            {
                _hasReachedCenter = true;
                return Status.Running;
            }

            // Move toward center point
            Vector2 direction = (_endPointTransform.position - _objectTransform.position).normalized;
            _objectTransform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            // Flip sprite based on facing direction (only once)
            if (direction.x != 0)
            {
                _objectTransform.localScale =
                    new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(_objectTransform.localScale.x),
                        _objectTransform.localScale.y, _objectTransform.localScale.z);
            }

            return Status.Running;
        }

        return Status.Success;
    }


    protected override void OnEnd()
    {
        _hasReachedCenter = false;
    }
}
