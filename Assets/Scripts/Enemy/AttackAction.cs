using System;
using Enemy;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Attack",
    story: "Shoot at [Player]",
    category: "Action",
    id: "21ad70ba8f1a94266d7fb966db76e5bc")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> ProjectilePrefab;
    [SerializeReference] public BlackboardVariable<Transform> FirePoint;
    [SerializeReference] public BlackboardVariable<float> LastAttackTime;

    [SerializeField] public float projectileSpeed = 5f;
    [SerializeField] public float cooldown = 2f; // seconds

    protected override Status OnUpdate()
    {
        if (Player?.Value == null || Enemy?.Value == null || ProjectilePrefab?.Value == null || FirePoint?.Value == null)
        {
            Debug.LogWarning("[AttackAction] Missing references.");
            return Status.Failure;
        }

        float timeSinceLastAttack = Time.time - LastAttackTime.Value;

        if (timeSinceLastAttack < cooldown)
        {
            return Status.Running; // Wait until cooldown finishes
        }

        ShootAtPlayer();
        LastAttackTime.Value = Time.time;
        return Status.Success;
    }

    private void ShootAtPlayer()
    {
        Vector2 direction = (Player.Value.transform.position - FirePoint.Value.position).normalized;

        GameObject projectile = UnityEngine.Object.Instantiate(
            ProjectilePrefab.Value,
            FirePoint.Value.position,
            Quaternion.identity);

        var projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Debug.Log("[AttackAction] Shot projectile at player.");
    }
}
