using Brutalsky;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerMovementController : SubControllerBase<BsPlayer>
    {
        public override string Id => "movement";
        public override bool IsUnused => false;

        public const int MaxOnGroundFrames = 5;

        public float movementForce;
        public float jumpForce;
        public bool dummy;
        public bool onGround;
        public float boostCharge;
        public float boostCooldown;
        private float _movementPush;
        private int _jumpCooldown;
        private bool _lastBoostInput;
        private Vector2 _lastPosition;
        private float _lastSpeed;
        private int _onGroundFrames;
        private float _lastPositionY;

        private Rigidbody2D _cRigidbody2D;
        private CircleCollider2D _cCircleCollider2D;

        public InputAction iMovement;
        public InputAction iBoost;
    
        protected override void OnInit()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();
            _cCircleCollider2D = GetComponent<CircleCollider2D>();

            iMovement = EventSystem._.inputActionAsset.FindAction("Movement");
            iMovement.Enable();
            iBoost = EventSystem._.inputActionAsset.FindAction("Boost");
            iBoost.Enable();

            // Disable movement if dummy is set to true
            dummy = Master.Object.Dummy;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision(other);

            // Get collision info
            if (!other.gameObject.CompareTag(Tags.Player)) return;
            var impactSpeed = _lastSpeed * other.DirectnessFactor();

            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) / 2f;
            _cRigidbody2D.velocity *= velocityFactor;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollision(other);
        }

        private void OnCollision(Collision2D other)
        {
            // Update ground status
            if (!other.gameObject.CompareTag(Tags.Shape) && !other.gameObject.CompareTag(Tags.Player)) return;
            if (other.GetContact(0).point.y > _lastPositionY - .25f) return;
            _onGroundFrames = MaxOnGroundFrames;
            onGround = true;
        }

        public void Freeze()
        {
            _cRigidbody2D.simulated = false;
            _cCircleCollider2D.enabled = false;
        }

        public void Unfreeze()
        {
            _cRigidbody2D.simulated = true;
            _cCircleCollider2D.enabled = true;
        }

        private void FixedUpdate()
        {
            if (dummy) return;

            // Update ground status
            onGround = _onGroundFrames > 0;
            _onGroundFrames = Mathf.Max(_onGroundFrames - 1, 0);
            _lastPositionY = transform.position.y;

            // Get movement data
            var velocity = _cRigidbody2D.velocity;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = iMovement.ReadValue<Vector2>();
            var jumpInput = onGround && movementInput.y > 0f && _jumpCooldown == 0;
            _cRigidbody2D.AddForce(movementInput * new Vector2(movementForce, Physics2D.gravity.magnitude * .5f));
            if (jumpInput)
            {
                _cRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpCooldown = MaxOnGroundFrames + 1;
            }

            // Apply boost movement
            var boostInput = iBoost.IsPressed() && boostCooldown == 0f;
            if (boostInput)
            {
                boostCharge = Mathf.Min(boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (_lastBoostInput)
            {
                var boost = Mathf.Pow(boostCharge, 1.5f) + 1.5f;
                velocity *= boost;
                _cRigidbody2D.velocity = velocity;
                boostCharge = 0f;
                boostCooldown = Mathf.Min(boost, speed);
            }
            if (boostCooldown > 0f)
            {
                boostCooldown = Mathf.Max(boostCooldown - Time.fixedDeltaTime, 0f);
            }
            _lastBoostInput = boostInput;

            // Save the current position and speed for future reference and update jump cooldown
            _lastPosition = transform.position;
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
            if (_jumpCooldown > 0)
            {
                _jumpCooldown--;
            }
        }
    }
}
