using System.Collections;
using System.Collections.Generic;
using Alcohol;
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
        Broken
    }
    
    public class Alcohol : MonoBehaviour
    {
        [SerializeField] EffectType alcoholEffect;
        [SerializeField] Sprite fullBottle;
        [SerializeField] Sprite halfBottle;
        [SerializeField] Sprite emptyBottle;
        [SerializeField] Sprite brokenBottle;
        [SerializeField] float cooldown = 25;
        [SerializeField] bool hasCooldown = true;

        private Effect _effect;
        public State state;
        private Image _image;
        private Coroutine _cooldownCoroutine;
        
        
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            state = State.Full;
            _image.sprite = GetSpriteByState();
            _effect = GetEffectByType();
            UpdateSprite();
        }
        
        private void UpdateSprite()
        {
            var newSprite = GetSpriteByState();
            _image.sprite = newSprite;
        }

        private Sprite GetSpriteByState()
        {
            switch (state)
            {
                case State.Full: return fullBottle;
                case State.Half: return halfBottle;
                case State.Empty: return emptyBottle;
                case State.Broken: default: return brokenBottle;
            }
        }
        public Sprite GetSpriteFull()
        {
            return fullBottle;
        }
        
        public Sprite GetSpriteEmpty()
        {
            return emptyBottle;
        }
        
        private Effect GetEffectByType()
        {
            Debug.Log(alcoholEffect);
            return alcoholEffect switch
            {
                EffectType.Swiftness => new Swiftness(),
                EffectType.Health => new Healing(),
                EffectType.Resistance => new Resistance(),
                EffectType.Power => new Power(),
                EffectType.Stun => new Stun(),
                _ => null
            };
        }

        public void ChangeState(State newState)
        {
            if (state == newState) return;
        
            state = newState;
            UpdateSprite();
        }
        
        public void Drink(PlayerController player)
        {
            Debug.Log(state);
            _effect = GetEffectByType();
            switch (state)
            {
                case State.Full:
                    ChangeState(State.Half);
                    _effect.ApplyWithDuration(player);
                    player.StartCooldown(cooldown);
                    return;
                case State.Half:
                    ChangeState(State.Empty);
                    _effect.ApplyWithDuration(player);
                    player.StartCooldown(cooldown);
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

        public void Repair()
        {
            Debug.Log("vsfgnhgtrefadsbghytewreafgjmyjtereafgbjmryet");
            state = State.Empty;
            UpdateSprite();
        }
    }
}