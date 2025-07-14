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
        [SerializeField] bool hasCooldown = true;

        private Effect _effect;
        public State state;
        private Image _image;
        private Coroutine _cooldownCoroutine;
        
        private float _cooldownTimer;
        private bool _isOnCooldown;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            state = State.Full;
            _image.sprite = GetSpriteByState();
            _effect = GetEffectByType();
            UpdateSprite();
        }
        
        private void Update()
        {
            if (_isOnCooldown)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0)
                {
                    Refill();
                    _isOnCooldown = false;
                }
            }
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
                case EffectType.Health: return new Healing();
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
        
        public void Drink(PlayerController player)
        {
            Debug.Log(state);
            _effect = GetEffectByType();
            switch (state)
            {
                case State.Full:
                    ChangeState(State.Half);
                    _effect.Apply(player);
                    return;
                case State.Half:
                    ChangeState(State.Empty);
                    _effect.Apply(player);
                    StartCooldown();
                    return;
                default:
                    Debug.LogWarning($"Can't drink in state: {state}");
                    return;
            }
        }
        
        private void StartCooldown()
        {
            _cooldownTimer = cooldown;
            _isOnCooldown = true;
        }
        
        public void Refill()
        {
            ChangeState(State.Full);
        }
    }
}