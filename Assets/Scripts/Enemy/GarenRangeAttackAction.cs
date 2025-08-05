using System;
using Managers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Garen Range Attack", story: "[Garen] fires sword waves", category: "Action", id: "b47b73f7f25eb38df598ed0877a0c8e7")]
public partial class GarenRangeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Garen;
    [SerializeReference] public BlackboardVariable<GameObject> Sword; // Prefab
    [SerializeReference] public BlackboardVariable<AudioClip> Yell2; // Target position
    [SerializeField] private float swordSpeed = 7f;
    [SerializeField] private int waveCount = 5;
    [SerializeField] private float timeBetweenWaves = 1.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float reachThreshold = 0.2f;
    private float _waveTimer = 0f;
    private int _wavesSpawned = 0;
    private Transform _garenTransform;
    private Transform _centerTransform;
    private bool _hasReachedCenter = false;
    private bool _spriteFlipState = false;
    private float _flipTimer = 0f;
    private float _flipInterval = 0.15f;

    private static readonly Vector2[] Directions = new[]
    {
        Vector2.left,
        Vector2.right,
        new Vector2(-1, 1).normalized,
        new Vector2(1, 1).normalized,
        Vector2.up
    };

    protected override Status OnStart()
    {
        if (Garen?.Value == null || Sword?.Value == null)
        {
            Debug.LogWarning("[GarenRangeAttack] Missing references.");
            return Status.Failure;
        }
        
        _garenTransform = Garen.Value.transform;
        _waveTimer = 0f;
        _wavesSpawned = 0;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        // Bullet hell part
        if (_wavesSpawned >= waveCount)
            return Status.Success;

        _waveTimer -= Time.deltaTime;
        if (_waveTimer <= 0f)
        {
            FireSwordWave();
            _waveTimer = timeBetweenWaves;
            _wavesSpawned++;
        }
        
        return Status.Running;
    }

    private void FireSwordWave()
    {
        foreach (var dir in Directions)
        {
            GameObject sword = GameObject.Instantiate(Sword.Value, _garenTransform.position, Quaternion.identity);
            var rb = sword.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = dir * swordSpeed;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Debug.Log($"[GarenRangeAttack] Fired wave {_wavesSpawned + 1}");
    }

    protected override void OnEnd()
    {
        _wavesSpawned = 0;
        _hasReachedCenter = false;
    }
}
