using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    enum EffectType
    {
        Swiftness,
        Health,
        Resistance,
        Power,
        Stun
    }
    
    public class Alcohol : MonoBehaviour
    {
        [SerializeField] EffectType alcoholEffect;
        [SerializeField] Sprite fullBottle;
        [SerializeField] Sprite halfBottle;
        [SerializeField] Sprite emptyBottle;
        [SerializeField] Sprite activatedBottle;
        [SerializeField] Sprite effectBottle;
        [SerializeField] Sprite brokenBottle;
        [SerializeField] float cooldown = 5;

        private Effect _effect;

        private void Awake()
        {
            _effect = GetEffectByType();
            gameObject.GetComponent<Image>().sprite = fullBottle;
        }

        private Effect GetEffectByType()
        {
            switch (alcoholEffect)
            {
                case EffectType.Swiftness:
                    return _effect = new Swiftness();
                case EffectType.Health:
                    return _effect = new Health();
                case EffectType.Resistance:
                    return _effect = new Resistance();
                case EffectType.Power:
                    return _effect = new Power();
                case EffectType.Stun:
                    return _effect = new Stun();
                default:
                    return null;
            }
        }
        
        public void Drink()
        {
            _effect = GetEffectByType();
            _effect.Apply();
        }
    }
}