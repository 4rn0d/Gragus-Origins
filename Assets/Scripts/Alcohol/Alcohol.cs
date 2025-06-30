using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts
{
    public enum EffectType
    {
        Swiftness,
        Health,
        Resistance,
        Power,
        Stun
    }

    public enum State
    {
        Full,
        Half,
        Empty,
        Broken,
        Activated,
        Effected
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
        public State state;
        private Image _image;

        private void Awake()
        {
            GetComponent<Image>().sprite = GetSpriteByState();
            _effect = GetEffectByType();
            Debug.Log($"Initial state: {state}");
            UpdateSprite();
        }
        
        private void UpdateSprite()
        {
            Debug.Log(state);
            var newSprite = GetSpriteByState();
            Debug.Log(newSprite);
            GetComponent<Image>().sprite = newSprite;
        }

        private Sprite GetSpriteByState()
        {
            switch (state)
            {
                case State.Full: return fullBottle;
                case State.Half: return halfBottle;
                case State.Empty: return emptyBottle;
                case State.Broken: return brokenBottle;
                case State.Activated: return activatedBottle;
                case State.Effected: return effectBottle;
                default: return fullBottle;
            }
        }
        
        private Effect GetEffectByType()
        {
            Debug.Log(alcoholEffect);   
            switch (alcoholEffect)
            {
                case EffectType.Swiftness: return new Swiftness();
                case EffectType.Health: return new Health();
                case EffectType.Resistance: return new Resistance();
                case EffectType.Power: return new Power();
                case EffectType.Stun: return new Stun();
                default: return null;
            }
        }

        public void ChangeState(State newState)
        {
            if (state == newState) return;
        
            state = newState;
            UpdateSprite();
        }
        
        public void Drink()
        {
            Debug.Log(state);
            _effect = GetEffectByType();
            switch (state)
            {
                case State.Full:
                    ChangeState(State.Half);
                    _effect.Apply();
                    return;
                case State.Half:
                    ChangeState(State.Empty);
                    _effect.Apply();
                    return;
                default:
                    Debug.LogWarning($"Can't drink in state: {state}");
                    return;
            }
        }
        
        public void Refill()
        {
            ChangeState(State.Full);
        }
    }
}