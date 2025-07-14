using System;
using System.Collections;
using Alcohol;
using Map;
using System.Threading;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Health;

namespace Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsFalling = Animator.StringToHash("IsFalling");
        private static readonly int IsRolling = Animator.StringToHash("IsRolling");
        private static readonly int IsDrinking = Animator.StringToHash("IsDrinking");


        [Header("Player Component References")] [SerializeField]
        Rigidbody2D rb;

        [SerializeField] Animator animator;

        [Header("Player Settings")] [SerializeField]
        float moveSpeed = 5f;

        [SerializeField] float acceleration = 10f;
        [SerializeField] float decceleration = 10f;
        [SerializeField] float velPower = 0.9f;
        [SerializeField] float maxAlcohoLevel = 10f;
        [SerializeField] float alcoholLevel = 10f;

        [Header("Jump Settings")] [SerializeField]
        float jumpForce = 7f;

        [SerializeField] float jumpMovementReductionForce = 0.14f;
        [SerializeField] float minJumpVelocity = 4f;
        [SerializeField] float coyoteTime = 0.2f;
        private float _coyoteTimeCounter;

        [Header("Dash Settings")] [SerializeField]
        float dashSpeed = 7.5f;

        [SerializeField] float dashDuration = 0.4f;
        [SerializeField] float dashDecceleration = 0.4f;
        [SerializeField] float bounceVerticalBoost = 5f;
        [SerializeField] float bounceHorizontalForce = 4f;
        [SerializeField] float bounceInputLockDuration = 0.6f;
        [SerializeField] AudioClip dashSound;

        [Header("BarrelThrow Settings")] [SerializeField]
        GameObject barrelPrefab;

        [SerializeField] Transform launchOffset;
        [SerializeField] float throwDistance = 0.5f;
        [SerializeField] float throwSpeed = 10f;
        [SerializeField] float explosionCooldown = 2f;
        [SerializeField] float explosionForce = 10f;
        [SerializeField] float throwAlcoholCost = 1f;
        private float _throwTimer;
        private float _explosionTimer;
        private bool _canThrow = true;
        private Barrel _barrel;

        [Header("Air Control")] [SerializeField]
        float fallAirControlMinMultiplier = 0.4f;

        [SerializeField] float fallAirControlMaxMultiplier = 1f;
        [SerializeField] float fallSpeedForMinControl = -2f;

        [Header("Other")] [SerializeField] Transform groundCheck;
        [SerializeField] Transform wallCheck;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] LayerMask wallLayer;
        [SerializeField] GameObject pauseMenuPrefab;
        private PauseMenu _pauseMenu;

        private float _horizontal;
        private float _lastNonZeroHorizontal = 1;

        private bool _isDashing;
        private float _dashTimer;
        private float _dashDirection;
        private bool _canDash = true;

        private bool _justBounced;
        private bool _isBouncing;
        private float _bounceTimer;
        private bool _hasBouncedThisDash;
        private float _rawHorizontalInput;

        //UI
        public Image _alcoholBar;
        private Alcohol _currentAlcohol;
        private AlcoholCarousel _alcoholCarousel;

        private bool _sModIsPressed;

        public bool triggerActive;

        private float _cooldownTimer;
        public bool isOnCooldown;
        public SpriteRenderer spriteRenderer;
        
        private Interactable _nearbyInteractible;
        
        public Health.PlayerHealth health;

        public bool healOnBarrel = false;
        void Awake()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _alcoholBar = GameObject.FindWithTag("AlcoholBar").GetComponent<Image>();
            _alcoholCarousel = GameObject.FindWithTag("AlcoholCarousel").GetComponent<AlcoholCarousel>();
            _pauseMenu = GameObject.FindWithTag("PauseMenu").GetComponent<PauseMenu>();
            _currentAlcohol = _alcoholCarousel.GetCurrentAlcohol();
            setAlcoolBarColor(Color.darkOrchid);

            health = gameObject.GetComponent<Health.PlayerHealth>();
        }
        private void FixedUpdate()
        {
            if (_pauseMenu.isPaused) return;
            UpdateFacingDirection();
            HandleThrow();
            HandleMovement();
            HandleDash();
            HandleBounceTimer();
            
        }
        
        private void Update()
        {
            if (!isOnCooldown) return;
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0)
            {
                isOnCooldown = false;
            }
            _justBounced = false;
        }

        private void HandleThrow()
        {
            if (!_canThrow)
            {
                _throwTimer -= Time.fixedDeltaTime;
                _explosionTimer -= Time.fixedDeltaTime;

                if (_throwTimer <= 0 && _barrel != null)
                {
                    _barrel.StopBarrel();
                }

                if (_explosionTimer <= 0 && _barrel != null)
                {
                    _barrel.ExplodeBarrel(this);
                    _canThrow = true;
                }
            }
        }

        public void BarrelJump(Vector3 position)
        {
            Vector3 moveDirection = position - rb.transform.position;
            rb.AddForce(moveDirection.normalized * -explosionForce, ForceMode2D.Impulse);
        }

        private void HandleMovement()
        {
            animator.SetFloat(Speed, Mathf.Abs(_horizontal));

            if (_isDashing || _isBouncing) return;

            float fallSpeed = rb.linearVelocity.y;
            float airControlMultiplier = 1f;

            if (!IsGrounded() && fallSpeed < 0)
            {
                float t = Mathf.InverseLerp(0f, fallSpeedForMinControl, fallSpeed);
                airControlMultiplier = Mathf.Lerp(fallAirControlMaxMultiplier, fallAirControlMinMultiplier, t);
                animator.SetBool(IsJumping, false);
            }

            if (IsGrounded())
            {
                _coyoteTimeCounter = coyoteTime;
            }
            else
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }

            animator.SetBool(IsJumping, !IsGrounded() && fallSpeed > 0.01f);
            animator.SetBool(IsFalling, !IsGrounded() && fallSpeed < -0.01f);

            float targetSpeed = _horizontal * moveSpeed * airControlMultiplier;
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
            rb.AddForce(movement * Vector2.right);
        }

        private void HandleDash()
        {
            if (!_isDashing) return;

            rb.linearVelocity = new Vector2(_dashDirection * dashSpeed, 0);
            _dashTimer -= Time.fixedDeltaTime;

            if (!_hasBouncedThisDash && IsTouchingWall())
            {
                TriggerBounce();
            }
            else if (_dashTimer <= 0)
            {
                EndDash();
            }
        }

        private void TriggerBounce()
        {
            _isBouncing = true;
            _bounceTimer = bounceInputLockDuration;
            _isDashing = false;
            _hasBouncedThisDash = true;
            _justBounced = true;

            float bounceRotation = 180f;
            if (_dashDirection == 1)
            {
                bounceRotation = 0f;
            }

            float bounceDir = -_dashDirection;
            transform.localRotation = new Quaternion(0f, bounceRotation, 0f, 1f);
            rb.linearVelocity = new Vector2(bounceDir * bounceHorizontalForce, bounceVerticalBoost);
            if (_rawHorizontalInput != 0)
                _lastNonZeroHorizontal = _rawHorizontalInput;
            else
                _lastNonZeroHorizontal = -_lastNonZeroHorizontal;
            
            
            animator.SetBool(IsDashing, false);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void EndDash()
        {
            
            animator.SetBool(IsDashing, false);
            _horizontal = _rawHorizontalInput;
            _isDashing = false;
            global::Health.Health playerHealth = GetComponent<global::Health.Health>();

            if (playerHealth != null)
            {
                Debug.Log("Vunerable");
                playerHealth.invulnerable = false;
            }
            _horizontal = _rawHorizontalInput;
            if (_rawHorizontalInput == 0)
            {
                float t = Mathf.InverseLerp(0f, fallSpeedForMinControl, rb.linearVelocity.y);
                float airControlMultiplier = Mathf.Lerp(fallAirControlMaxMultiplier, fallAirControlMinMultiplier, t);
                float targetSpeed = _horizontal * moveSpeed * airControlMultiplier;
                float speedDiff = targetSpeed - rb.linearVelocity.x;
                float movement = Mathf.Pow(Mathf.Abs(speedDiff) * (dashDecceleration), velPower) *
                                 Mathf.Sign(speedDiff);
                rb.AddForce(movement * Vector2.right);
                _horizontal = 0;
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
            }
        }

        private void HandleBounceTimer()
        {
            if (_isBouncing)
            {
                _bounceTimer -= Time.fixedDeltaTime;
                if (_bounceTimer <= 0)
                {
                    _isBouncing = false;
                    _horizontal = _rawHorizontalInput;
                }
            }

            if (IsGrounded() || _hasBouncedThisDash && !_isDashing && !_isBouncing)
            {
                _canDash = true;
            }
        }
        private float GetCurrentDirection()
        {
            if (Mathf.Abs(rb.linearVelocity.x) > 0.2f)
            {
                return Mathf.Sign(rb.linearVelocity.x);
            }
            return _lastNonZeroHorizontal;
        }


        private void UpdateFacingDirection()
        {
            transform.localRotation = new Quaternion(0f, (Mathf.Sign(GetCurrentDirection()) * -180) - 180f, 0f, 1f);
        }

        public bool IsGrounded()
        {
            return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.2f),
                CapsuleDirection2D.Horizontal, 0, groundLayer);
        }

        public bool IsTouchingWall()
        {
            float direction = _isDashing ? _dashDirection : Mathf.Sign(_horizontal);
            return Physics2D.Raycast(wallCheck.position, Vector2.right * direction, 0.3f, wallLayer);
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (context.performed && !_pauseMenu.isPaused && _coyoteTimeCounter > 0)
            {
                animator.SetBool(IsJumping, true);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            if (context.canceled && rb.linearVelocity.y > minJumpVelocity)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, minJumpVelocity);
                _coyoteTimeCounter = 0f;
            }
        }

        public void Dash(InputAction.CallbackContext context)
        {
            if (context.performed && !_isDashing && _canDash && !_pauseMenu.isPaused)
            {
                SoundFXManager.instance.PlaySoundFXClip(dashSound, transform, 1f);
                animator.SetBool(IsDashing, true);
                _isDashing = true;
                
                global::Health.Health playerHealth = GetComponent<global::Health.Health>();

                if (playerHealth != null)
                {
                    playerHealth.invulnerable = true;
                    Debug.Log("Invulnerable");
                }
                
                _dashTimer = dashDuration;
                _dashDirection = _lastNonZeroHorizontal;
                _hasBouncedThisDash = false;
                _canDash = false;
            }
        }

        public void Move(InputAction.CallbackContext context)
        {
            float input = context.ReadValue<Vector2>().x;
            _rawHorizontalInput = input;

            if (input != 0)
                _lastNonZeroHorizontal = Mathf.Sign(input);

            if (!_isDashing)
            {
                _horizontal = input;
            }
        }

        public void SMod(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _sModIsPressed = context.performed;
            }
        }

        public void StartBarrelAnim(InputAction.CallbackContext context)
        {
            if (context.performed && !_pauseMenu.isPaused)
            {
                if (_canThrow)
                {
                    if (alcoholLevel - throwAlcoholCost >= 0)
                    {
                        _canThrow = false;
                        animator.SetBool(IsRolling, true);
                    }
                }
                else
                {
                    Debug.Log("Boom");
                    _barrel.ExplodeBarrel(this);
                    _canThrow = true;
                }
            }
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            Debug.Log("TogglePause performed");
            if (context.performed)
            {
                Debug.Log("TogglePause performed");
                _pauseMenu.TogglePause();
            }
        }

        private float UseAlcohol(float cost)
        {
            alcoholLevel -= cost;
            _alcoholBar.fillAmount -= (cost * 0.1f);
            Debug.Log(alcoholLevel);
            return alcoholLevel;
        }

        public void DrinkAlcohol(InputAction.CallbackContext context)
        {
            if (context.performed && !_pauseMenu.isPaused)
            {
                Debug.Log(_currentAlcohol);
                if (!isOnCooldown && (_currentAlcohol.state != State.Broken && _currentAlcohol.state != State.Empty))
                {
                    animator.SetBool(IsDrinking, true);
                    _currentAlcohol.Drink(this);  
                }
                else
                { 
                    Debug.Log("AlcoolOnCooldown");
                }
            }
        }

        public void StartCooldown(float cooldown)
        {
            _cooldownTimer = cooldown;
            isOnCooldown = true;
        }
        public void ChangeAlcohol(InputAction.CallbackContext context)
        {
            if (context.performed && !_pauseMenu.isPaused)
            {
                _currentAlcohol = _alcoholCarousel.NextPotion();
                // Debug.Log("TogglePause performed");
                // _pauseMenu.TogglePause();
            }
        }

        private void EndBarrelAnim()
        {
            animator.SetBool(IsRolling, false);
        }

        private void EndDrinkingAnim()
        {
            animator.SetBool(IsDrinking, false);
        }

        private void SpawnBarrel()
        {
            health.AddHealth(50);
            UseAlcohol(throwAlcoholCost);
            _throwTimer = throwDistance;
            _explosionTimer = explosionCooldown;
            if (_sModIsPressed)
            {
                _barrel = Instantiate(barrelPrefab, transform.position, transform.rotation).GetComponent<Barrel>();
            }
            else
            {
                _barrel = Instantiate(barrelPrefab, launchOffset.position, launchOffset.rotation)
                    .GetComponent<Barrel>();
                _barrel.InitalizeBarrel(rb, throwSpeed);
            }
        }

        public bool IsThePlayerDashing()
        {
            return _isDashing;
        }
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (_nearbyInteractible != null && !_nearbyInteractible.IsUsed)
            {
                _nearbyInteractible.Interact(this);
            }
        }


        public void RefillAlcohol(float amount)
        {
            alcoholLevel = Mathf.Clamp(alcoholLevel + amount, 0f, maxAlcohoLevel);
            _alcoholBar.fillAmount = alcoholLevel / maxAlcohoLevel;
            Debug.Log($"[Fountain] Refilled alcohol. Current level: {alcoholLevel}");
        }

        public void SetNearbyInteractable(Interactable interactable)
        {
            _nearbyInteractible = interactable;
        }

        public void ClearNearbyInteractable(Interactable interactable)
        {
            if (_nearbyInteractible == interactable)
                _nearbyInteractible = null;
        }

        public void setSpeed(float speed)
        {
            moveSpeed += speed;
        }

        public void setAlcoolBarColor(Color color)
        {
            _alcoholBar.color = color;
        }

    }

}

