using Scripts;
using UnityEngine;

namespace Alcohol
{
    public abstract class Effect
    {
        private const float _duration = 5f;

        public void ApplyWithDuration(PlayerController player)
        {
            Apply(player);
            player.StartCoroutine(EffectDuration(player));
        }

        public abstract void Apply(PlayerController player);

        protected abstract void Revert(PlayerController player);

        private System.Collections.IEnumerator EffectDuration(PlayerController player)
        {
            yield return new WaitForSeconds(_duration);
            Revert(player);
        }
    }

    public class Swiftness : Effect
    {
        public override void Apply(PlayerController player)
        {
            player.setSpeed(2);
            player.setAlcoolBarColor(Color.deepSkyBlue);
        }

        protected override void Revert(PlayerController player)
        {
            player.setSpeed(-2);
            player.setAlcoolBarColor(Color.darkOrchid);
        }
    }

    public class Healing : Effect
    {
        public override void Apply(PlayerController player)
        {
            player.healOnBarrel = true;
            player.setAlcoolBarColor(Color.darkRed);
        }

        protected override void Revert(PlayerController player)
        {
            player.healOnBarrel = false;
            player.setAlcoolBarColor(Color.darkOrchid);
        }
    }
    
    public class Resistance : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Resistance");
        }

        protected override void Revert(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class Power : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Power");
        }

        protected override void Revert(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class Stun : Effect
    {
        public override void Apply(PlayerController player)
        {
            Debug.Log("Stun");
        }

        protected override void Revert(PlayerController player)
        {
            throw new System.NotImplementedException();
        }
    }
}