using UnityEngine;
using Utils.Ext;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Variables
        public float movementForce;
        public float jumpForce;
        public bool dummy;
        private float _movementPush;
        private int _jumpCooldown;
        private bool _lastBoostInput;
        private Vector2 _lastPosition;
        private float _lastSpeed;

        // References
        private PlayerController _cPlayerController;
        private Rigidbody2D _cRigidbody2D;
    
        // Events
        private void Start()
        {
            _cPlayerController = GetComponent<PlayerController>();
            _cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (!other.gameObject.CompareTag(PlayerController.Tag)) return;
            var impactSpeed = _lastSpeed * other.DirectnessFactor();

            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) / 2f;
            _cRigidbody2D.velocity *= velocityFactor;
        }

        // Updates
        private void FixedUpdate()
        {
            if (dummy) return;

            // Get movement data
            var position = (Vector2)transform.position;
            //// This works because glue causes rapid vibration,
            //// resulting in actual velocity being much higher than derived velocity.
            var derivedVelocity = (position - _lastPosition) / Time.fixedDeltaTime;
            var playerStuck = derivedVelocity.magnitude < .1f && _cRigidbody2D.velocity.magnitude > .1f;
            var velocity = !playerStuck ? _cRigidbody2D.velocity : Vector2.zero;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = _cPlayerController.iMovement.ReadValue<Vector2>();
            var jumpInput = _cPlayerController.onGround && movementInput.y > 0f && _jumpCooldown == 0;
            _movementPush = playerStuck && movementInput.magnitude > 0f
                ? _movementPush + Time.fixedDeltaTime
                : Mathf.Max(_movementPush - speed * Time.fixedDeltaTime, 1f);
            movementInput *= _movementPush;
            _cRigidbody2D.AddForce(movementInput * new Vector2(movementForce, Physics2D.gravity.magnitude * .5f));
            if (jumpInput)
            {
                _cRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _jumpCooldown = PlayerController.MaxOnGroundFrames + 1;
            }

            // Apply boost movement
            var boostInput = _cPlayerController.iBoost.IsPressed() && _cPlayerController.boostCooldown == 0f;
            if (boostInput)
            {
                _cPlayerController.boostCharge = Mathf.Min(_cPlayerController.boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (_lastBoostInput)
            {
                var boost = Mathf.Pow(_cPlayerController.boostCharge, 1.5f) + 1.5f;
                velocity *= boost;
                _cRigidbody2D.velocity = velocity;
                _cPlayerController.boostCharge = 0f;
                _cPlayerController.boostCooldown = Mathf.Min(boost, speed);
            }
            if (_cPlayerController.boostCooldown > 0f)
            {
                _cPlayerController.boostCooldown = Mathf.Max(_cPlayerController.boostCooldown - Time.fixedDeltaTime, 0f);
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
