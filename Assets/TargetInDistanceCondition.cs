using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "TargetInDistance",
    story: "[Target] is in [proximity] to [Enemy]",
    category: "Conditions",
    id: "44394ca630ff451092a2fc59fab3eec1")]
public partial class TargetInDistanceCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Proximity;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    public override bool IsTrue()
    {
        if (Target?.Value == null || Enemy?.Value == null)
            return false;

        float distance = Vector2.Distance(Target.Value.transform.position, Enemy.Value.transform.position);
        Debug.Log($"Distance: {distance}, Proximity: {Proximity.Value}");
        return distance <= Proximity.Value;
    }
}