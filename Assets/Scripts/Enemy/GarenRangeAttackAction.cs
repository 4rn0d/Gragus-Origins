using System;
using Managers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Garen Range Attack", story: "[Garen] walks to center and fires sword waves", category: "Action", id: "b47b73f7f25eb38df598ed0877a0c8e6")]
public partial class GarenRangeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Garen;
    [SerializeReference] public BlackboardVariable<GameObject> Sword; // Prefab
    [SerializeReference] public BlackboardVariable<GameObject> CenterPoint; // Target position
    [SerializeReference] public BlackboardVariable<AudioClip> Yell2; // Target position
    [SerializeField] private float swordSpeed = 7f;
    [SerializeField] private int waveCount = 5;
    [SerializeField] private float timeBetweenWaves = 1.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float reachThreshold = 0.2f;

    private Animator _animator;
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
        if (Garen?.Value == null || Sword?.Value == null || CenterPoint?.Value == null)
        {
            Debug.LogWarning("[GarenRangeAttack] Missing references.");
            return Status.Failure;
        }
        
        _animator = Garen.Value.GetComponent<Animator>();

        _garenTransform = Garen.Value.transform;
        _centerTransform = CenterPoint.Value.transform;
        _waveTimer = 0f;
        _wavesSpawned = 0;
        _hasReachedCenter = false;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!_hasReachedCenter)
        {
            float distance = Vector2.Distance(_garenTransform.position, _centerTransform.position);
            if (distance <= reachThreshold)
            {
                _hasReachedCenter = true;
                SoundFXManager.instance.PlaySoundFXClip(Yell2.Value,_garenTransform,1f);
                return Status.Running;
            }

            // Move toward center point
            Vector2 direction = (_centerTransform.position - _garenTransform.position).normalized;
            _garenTransform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            // Flip sprite based on facing direction (only once)
            if (direction.x != 0)
            {
                _garenTransform.localScale = new Vector3(Mathf.Sign(direction.x) * Mathf.Abs(_garenTransform.localScale.x), _garenTransform.localScale.y, _garenTransform.localScale.z);
            }

            return Status.Running;
        }

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
            GameObject sword = UnityEngine.Object.Instantiate(Sword.Value, _garenTransform.position, Quaternion.identity);
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
