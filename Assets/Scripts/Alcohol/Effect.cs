using Scripts;
using UnityEngine;

namespace Alcohol
{
    public abstract class Effect
    {
        public abstract void Apply(PlayerController player);
    }

    public class Swiftness : Effect
    {
        public override void Apply(PlayerController player)
        {
            player.setSpeed();
        }
    }

    public class Healing : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Health");
        }
    }
    
    public class Resistance : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Resistance");
        }
    }
    
    public class Power : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Power");
        }
    }
    
    public class Stun : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Stun");
        }
    }
}