using UnityEngine;

namespace Scripts
{
    public abstract class Effect
    {
        public abstract void Apply();
    }

    public class Swiftness : Effect
    {
        public override void Apply()
        {
            Debug.Log("Swiftness");
        }
    }

    public class Health : Effect
    {
        public override void Apply()
        {
            Debug.Log("Health");
        }
    }
    
    public class Resistance : Effect
    {
        public override void Apply()
        {
            Debug.Log("Resistance");
        }
    }
    
    public class Power : Effect
    {
        public override void Apply()
        {
            Debug.Log("Power");
        }
    }
    
    public class Stun : Effect
    {
        public override void Apply()
        {
            Debug.Log("Stun");
        }
    }
}