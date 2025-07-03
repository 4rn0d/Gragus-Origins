using System;
using Unity.Behavior;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "Player In Range",
    story: "[Player] is within [Range] of [Enemy]",
    category: "Conditions")]
public partial class PlayerInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        if (Player?.Value == null || Enemy?.Value == null)
            return false;

        float distance = Vector2.Distance(
            Player.Value.transform.position,
            Enemy.Value.transform.position);

        Debug.Log($"[Condition] Distance: {distance}, Range: {Range.Value}");

        if (distance <= Range.Value)
        {
            Debug.Log("Player In Range");
            return true;
        }

        Debug.Log("Player Not In Range");
        
        return false;
    }
}