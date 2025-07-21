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
            player.resistant = true;
            player.setAlcoolBarColor(Color.orange);
        }

        protected override void Revert(PlayerController player)
        {
            player.healOnBarrel = false;
            player.setAlcoolBarColor(Color.darkOrchid);
        }
    }
    
    public class Power : Effect
    {
        public override void Apply(PlayerController player)
        {
            player.powerful = true;
            player.setAlcoolBarColor(Color.darkMagenta);
        }

        protected override void Revert(PlayerController player)
        {
            player.powerful = false;
            player.setAlcoolBarColor(Color.darkOrchid);
        }
    }
    
    public class Stun : Effect
    {
        public override void Apply(PlayerController player)
        {
            player.sticky = true;
            player.setAlcoolBarColor(Color.pink);
        }

        protected override void Revert(PlayerController player)
        {
            player.sticky = false;
            player.setAlcoolBarColor(Color.darkOrchid);
        }
    }
}