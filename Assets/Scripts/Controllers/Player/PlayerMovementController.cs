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
        private float movementPush;
        private int jumpCooldown;
        private bool lastBoostInput;
        private Vector2 lastPosition;
        private float lastSpeed;

        // References
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;
    
        // Events
        private void Start()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            if (!other.gameObject.CompareTag(PlayerController.Tag)) return;
            var impactSpeed = lastSpeed * other.DirectnessFactor();

            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) / 2f;
            cRigidbody2D.velocity *= velocityFactor;
        }

        // Updates
        private void FixedUpdate()
        {
            if (dummy) return;

            // Get movement data
            var position = (Vector2)transform.position;
            //// This works because glue causes rapid vibration,
            //// resulting in actual velocity being much higher than derived velocity.
            var derivedVelocity = (position - lastPosition) / Time.fixedDeltaTime;
            var playerStuck = derivedVelocity.magnitude < .1f && cRigidbody2D.velocity.magnitude > .1f;
            var velocity = !playerStuck ? cRigidbody2D.velocity : Vector2.zero;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = cPlayerController.iMovement.ReadValue<Vector2>();
            var jumpInput = cPlayerController.onGround && movementInput.y > 0f && jumpCooldown == 0;
            if (jumpInput)
            {
                cRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpCooldown = PlayerController.MaxOnGroundFrames + 1;
            }
            movementPush = playerStuck && movementInput.magnitude > 0f
                ? movementPush + Time.fixedDeltaTime
                : Mathf.Max(movementPush - speed * Time.fixedDeltaTime, 1f);
            movementInput *= movementPush;
            cRigidbody2D.AddForce(movementInput * new Vector2(movementForce, Physics2D.gravity.magnitude * .5f));

            // Apply boost movement
            var boostInput = cPlayerController.iBoost.IsPressed() && cPlayerController.boostCooldown == 0f;
            if (boostInput)
            {
                cPlayerController.boostCharge = Mathf.Min(cPlayerController.boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (lastBoostInput)
            {
                var boost = Mathf.Pow(cPlayerController.boostCharge, 1.5f) + 1.5f;
                velocity *= boost;
                cRigidbody2D.velocity = velocity;
                cPlayerController.boostCharge = 0f;
                cPlayerController.boostCooldown = Mathf.Min(boost, speed);
            }
            if (cPlayerController.boostCooldown > 0f)
            {
                cPlayerController.boostCooldown = Mathf.Max(cPlayerController.boostCooldown - Time.fixedDeltaTime, 0f);
            }
            lastBoostInput = boostInput;

            // Save the current position and speed for future reference and update jump cooldown
            lastPosition = transform.position;
            lastSpeed = cRigidbody2D.velocity.magnitude;
            if (jumpCooldown > 0)
            {
                jumpCooldown--;
            }
        }
    }
}
