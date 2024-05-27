using System;
using Controllers.Base;
using Data;
using Data.Object;
using Systems;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerMovementController : SubControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Id => "movement";
        public override bool IsUnused => false;

        // Config options
        public float movementForce;
        public float jumpForce;
        public int maxGroundedFrames;

        // Exposed properties
        public bool Dummy { get; private set; }
        public bool Grounded { get; private set; }
        public float BoostCharge { get; private set; }
        public float BoostCooldown { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public bool BoostInput { get; private set; }

        // Local variables
        private Vector2 _movementScale;
        private Vector2 _jumpVector;
        private int _jumpCooldown;
        private bool _lastBoostInput;
        private float _lastSpeed;
        private int _groundedFrames;
        private float _groundFriction;
        private Vector2 _lastPosition;

        // Local functions
        public Func<Vector2, Vector2, bool> TestGrounded = (_, _) => false;
        public Func<Vector2, bool> TestJumpInput = _ => false;

        // External references
        private Rigidbody2D _cRigidbody2D;

        // Init functions
        protected override void OnInit()
        {
            EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
            EventSystem._.OnMapBuild += OnMapBuild;

            _cRigidbody2D = GetComponent<Rigidbody2D>();

            // Disable movement if the player is a dummy
            Dummy = Master.Object.Type == BsPlayer.TypeDummy;

            // Automatically grab map settings if there is a loaded map
            if (MapSystem._.MapLoaded)
            {
                OnMapBuild(MapSystem._.ActiveMap);
            }
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
            EventSystem._.OnMapBuild -= OnMapBuild;
        }

        // System functions
        public void SendInput(Vector2 movement, bool boost)
        {
            MovementInput = movement;
            BoostInput = boost;
        }

        // Event functions
        private void OnMapBuild(BsMap map)
        {
            // Configure movement to fit with current map settings
            _movementScale = map.GravityDirection switch
            {
                BsMap.DirectionDown => new Vector2(movementForce, map.GravityStrength * .5f),
                BsMap.DirectionUp => new Vector2(movementForce, map.GravityStrength * .5f),
                BsMap.DirectionLeft => new Vector2(map.GravityStrength * .5f, movementForce),
                BsMap.DirectionRight => new Vector2(map.GravityStrength * .5f, movementForce),
                _ => Vector2.one * movementForce
            };
            _jumpVector = map.GravityDirection switch
            {
                BsMap.DirectionDown => Vector2.up * jumpForce,
                BsMap.DirectionUp => Vector2.down * jumpForce,
                BsMap.DirectionLeft => Vector2.right * jumpForce,
                BsMap.DirectionRight => Vector2.left * jumpForce,
                _ => Vector2.zero
            };
            TestGrounded = map.GravityDirection switch
            {
                BsMap.DirectionDown => (c, p) => c.y < p.y - .25f,
                BsMap.DirectionUp => (c, p) => c.y > p.y + .25f,
                BsMap.DirectionLeft => (c, p) => c.x < p.x - .25f,
                BsMap.DirectionRight => (c, p) => c.x > p.x + .25f,
                _ => (_, _) => false
            };
            TestJumpInput = map.GravityDirection switch
            {
                BsMap.DirectionDown => m => m.y > 0f,
                BsMap.DirectionUp => m => m.y < 0f,
                BsMap.DirectionLeft => m => m.x > 0f,
                BsMap.DirectionRight => m => m.x < 0f,
                _ => _ => false
            };
            _cRigidbody2D.drag = map.AirResistance;
        }

        private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
        {
            if (player.Id != Master.Object.Id) return;
            _cRigidbody2D.velocity = Vector2.zero;
            BoostCharge = 0f;
            BoostCooldown = 0f;
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
            var isShape = other.gameObject.CompareTag(Tags.ShapeTag);
            var isPlayer = other.gameObject.CompareTag(Tags.PlayerTag);
            if (!isShape && !isPlayer) return;
            if (!TestGrounded(other.GetContact(0).point, _lastPosition)) return;
            _groundedFrames = maxGroundedFrames;
            _groundFriction = isShape
                ? other.gameObject.GetComponent<PolygonCollider2D>().sharedMaterial.friction
                : .1f;
            Grounded = true;
        }

        private void FixedUpdate()
        {
            if (Dummy) return;

            // Update ground status
            _groundedFrames = Mathf.Max(_groundedFrames - 1, 0);
            Grounded = _groundedFrames > 0;

            // Get movement data
            var velocity = _cRigidbody2D.velocity;
            var speed = velocity.magnitude;

            // Apply directional movement
            _cRigidbody2D.AddForce(MovementInput * _movementScale * (Grounded ? Mathf.Clamp(_groundFriction, .5f, 2f) : .5f));

            // Apply jump movement
            var jumpInput = Grounded && TestJumpInput(MovementInput) && _jumpCooldown == 0;
            if (jumpInput)
            {
                _cRigidbody2D.AddForce(_jumpVector, ForceMode2D.Impulse);
                _jumpCooldown = maxGroundedFrames;
            }
            if (_jumpCooldown > 0)
            {
                _jumpCooldown--;
            }

            // Apply boost movement
            var safeBoostInput = BoostInput && BoostCooldown == 0f;
            if (safeBoostInput)
            {
                BoostCharge = Mathf.Min(BoostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (_lastBoostInput)
            {
                var boost = Mathf.Pow(BoostCharge, 1.5f) + 1.5f;
                velocity *= boost;
                _cRigidbody2D.velocity = velocity;
                BoostCharge = 0f;
                BoostCooldown = Mathf.Min(boost, speed);
            }
            if (BoostCooldown > 0f)
            {
                BoostCooldown = Mathf.Max(BoostCooldown - Time.fixedDeltaTime, 0f);
            }
            _lastBoostInput = safeBoostInput;

            // Save the current position and speed for future reference
            _lastPosition = transform.localPosition;
            _lastSpeed = _cRigidbody2D.velocity.magnitude;
        }
    }
}
