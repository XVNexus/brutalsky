using System;
using Brutalsky;
using Controllers.Base;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerMovementController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "movement";
        public override bool IsUnused => false;

        // Local constants
        public const int MaxOnGroundFrames = 5;

        // Local variables
        public float movementForce;
        public float jumpForce;
        public bool dummy;
        public bool onGround;
        public float boostCharge;
        public float boostCooldown;
        private Vector2 _movementScale;
        private Vector2 _jumpVector;
        private int _jumpCooldown;
        private bool _lastBoostInput;
        private float _lastSpeed;
        private int _onGroundFrames;
        private Vector2 _lastPosition;

        // Local functions
        public Func<Vector2, Vector2, bool> TestOnGround = (_, _) => false;
        public Func<Vector2, bool> TestJumpInput = _ => false;

        // Component references
        private Rigidbody2D _cRigidbody2D;

        // Player input
        public InputAction iMovement;
        public InputAction iBoost;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnMapBuild += OnMapBuild;
            EventSystem._.OnPlayerRespawn += OnPlayerRespawn;

            _cRigidbody2D = GetComponent<Rigidbody2D>();

            iMovement = EventSystem._.aInputAction.FindAction("Movement");
            iMovement.Enable();
            iBoost = EventSystem._.aInputAction.FindAction("Boost");
            iBoost.Enable();

            // Disable movement if dummy is set to true
            dummy = Master.Object.Dummy;

            // Force scanning map settings
            OnMapBuild(MapSystem._.ActiveMap);
        }

        // Event functions
        private void OnMapBuild(BsMap map)
        {
            // Configure movement to fit with current gravity
            _movementScale = map.GravityDirection switch
            {
                Direction.Down => new Vector2(movementForce, map.GravityStrength * .5f),
                Direction.Up => new Vector2(movementForce, map.GravityStrength * .5f),
                Direction.Left => new Vector2(map.GravityStrength * .5f, movementForce),
                Direction.Right => new Vector2(map.GravityStrength * .5f, movementForce),
                _ => Vector2.one * movementForce
            };
            _jumpVector = map.GravityDirection switch
            {
                Direction.Down => Vector2.up * jumpForce,
                Direction.Up => Vector2.down * jumpForce,
                Direction.Left => Vector2.right * jumpForce,
                Direction.Right => Vector2.left * jumpForce,
                _ => Vector2.zero
            };
            TestOnGround = map.GravityDirection switch
            {
                Direction.Down => (c, p) => c.y < p.y - .25f,
                Direction.Up => (c, p) => c.y > p.y + .25f,
                Direction.Left => (c, p) => c.x < p.x - .25f,
                Direction.Right => (c, p) => c.x > p.x + .25f,
                _ => (_, _) => false
            };
            TestJumpInput = map.GravityDirection switch
            {
                Direction.Down => m => m.y > 0f,
                Direction.Up => m => m.y < 0f,
                Direction.Left => m => m.x > 0f,
                Direction.Right => m => m.x < 0f,
                _ => _ => false
            };
        }

        private void OnPlayerRespawn(BsMap map, BsPlayer player)
        {
            if (player.Id != Master.Object.Id) return;
            _cRigidbody2D.velocity = Vector2.zero;
            boostCharge = 0f;
            boostCooldown = 0f;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision(other);

            // Get collision info
            if (!other.gameObject.CompareTag(Tags.PlayerTag) || other.relativeVelocity.magnitude < 5f) return;
            var impactSpeed = _lastSpeed * other.DirectnessFactor();

            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) * .5f;
            _cRigidbody2D.velocity *= velocityFactor;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollision(other);
        }

        private void OnCollision(Collision2D other)
        {
            // Update ground status
            if (!other.gameObject.CompareTag(Tags.ShapeTag) && !other.gameObject.CompareTag(Tags.PlayerTag)) return;
            if (!TestOnGround(other.GetContact(0).point, _lastPosition)) return;
            _onGroundFrames = MaxOnGroundFrames;
            onGround = true;
        }

        private void FixedUpdate()
        {
            if (dummy) return;

            // Update ground status
            onGround = _onGroundFrames > 0;
            _onGroundFrames = Mathf.Max(_onGroundFrames - 1, 0);
            _lastPosition = transform.position;

            // Get movement data
            var velocity = _cRigidbody2D.velocity;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = iMovement.ReadValue<Vector2>();
            _cRigidbody2D.AddForce(movementInput * _movementScale * (onGround ? 1.5f : .5f));

            // Apply jump movement
            var jumpInput = onGround && TestJumpInput(movementInput) && _jumpCooldown == 0;
            if (jumpInput)
            {
                _cRigidbody2D.AddForce(_jumpVector, ForceMode2D.Impulse);
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

            // Save the current speed for future reference and update jump cooldown
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
            if (_jumpCooldown > 0)
            {
                _jumpCooldown--;
            }
        }
    }
}
