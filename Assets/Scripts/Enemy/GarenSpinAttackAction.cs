using System;
using System.Collections.Generic;
using Managers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Garen Spin Attack", story: "[Garen] [spins] and [yell] [around] the arena to damage [player]",
    category: "Action", id: "6effff757106ce8c604f417a3ef9ae7e")]
public partial class GarenSpinAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Garen;
    [SerializeReference] public BlackboardVariable<AudioClip> Spins;
    [SerializeReference] public BlackboardVariable<AudioClip> Yell;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Around;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private int spinCountTarget = 2;
    [SerializeField] private float flipRate = 0.1f; // How fast he flips (lower = faster)

    private Transform _garenTransform;
    private Vector3 _originalScale;
    private int _currentTargetIndex = 0;
    private int _lapCounter = 0;

    private float _flipTimer = 0f;
    private bool _flipped = false;

    protected override Status OnStart()
    {
        if (Garen?.Value == null || Around?.Value == null || Around.Value.Count < 2)
        {
            Debug.LogWarning("[GarenSpinAttack] Missing required references or waypoints.");
            return Status.Failure;
        }

        _garenTransform = Garen.Value.transform;
        _originalScale = _garenTransform.localScale;
        _currentTargetIndex = 0;
        _lapCounter = 0;
        _flipTimer = 0f;
        _flipped = false;

        SoundFXManager.instance.PlaySoundFXClip(Yell.Value, _garenTransform, 1f);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_garenTransform == null || Around?.Value == null || Around.Value.Count < 2)
            return Status.Failure;

        Vector3 targetPos = Around.Value[_currentTargetIndex].transform.position;
        Vector3 currentPos = _garenTransform.position;

        // Move toward target
        _garenTransform.position = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

        // Flip the sprite left and right to simulate spinning
        _flipTimer -= Time.deltaTime;
        if (_flipTimer <= 0f)
        {
            _flipped = !_flipped;
            _garenTransform.localScale = new Vector3(
                (_flipped ? -1 : 1) * Mathf.Abs(_originalScale.x),
                _originalScale.y,
                _originalScale.z
            );
            _flipTimer = flipRate;
        }

        // Check if reached target point
        if (Vector3.Distance(currentPos, targetPos) <= distanceThreshold)
        {
            SoundFXManager.instance.PlaySoundFXClip(Spins, _garenTransform, 1f);

            _currentTargetIndex = (_currentTargetIndex + 1) % 2;

            if (_currentTargetIndex == 0)
            {
                _lapCounter++;
                Debug.Log($"[GarenSpinAttack] Lap {_lapCounter} complete.");
            }

            if (_lapCounter >= spinCountTarget)
            {
                return Status.Success;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _lapCounter = 0;
        _currentTargetIndex = 0;
        _garenTransform.localScale = _originalScale;
    }
}