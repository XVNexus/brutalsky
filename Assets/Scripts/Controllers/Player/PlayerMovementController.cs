using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Constants
        public const int MaxOnGroundFrames = 3;

        // Variables
        public float movementForce = 20f;
        private int onGroundFrames;
        private bool onGround;
        private float movementPush;
        private bool lastBoostInput;
        private Vector2 lastPosition;
        private float lastSpeed;

        // References
        private PlayerController cPlayerController;
        private Rigidbody2D cRigidbody2D;
        private InputAction iMovement;
        private InputAction iBoost;
    
        // Events
        private void OnEnable()
        {
            cPlayerController = GetComponent<PlayerController>();
            cRigidbody2D = GetComponent<Rigidbody2D>();

            iMovement = EventSystem.current.inputActionAsset.FindAction("Movement");
            iMovement.Enable();
            iBoost = EventSystem.current.inputActionAsset.FindAction("Boost");
            iBoost.Enable();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision(other);

            // Get collision info
            if (!other.gameObject.CompareTag("Player")) return;
            var impactSpeed = lastSpeed;

            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) / 2f;
            cRigidbody2D.velocity *= velocityFactor;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollision(other);
        }

        private void OnCollision(Collision2D other)
        {
            // Update ground status
            if (!other.gameObject.CompareTag("Shape") && !other.gameObject.CompareTag("Player")) return;
            if (!(other.GetContact(0).point.y < transform.position.y - .25f)) return;
            onGroundFrames = MaxOnGroundFrames;
            onGround = true;
        }

        // Updates
        private void FixedUpdate()
        {
            // Get movement data
            var position = (Vector2)transform.position;
            //// This works because glue causes rapid vibration,
            //// resulting in actual velocity being much higher than derived velocity.
            var derivedVelocity = (position - lastPosition) / Time.fixedDeltaTime;
            var playerStuck = derivedVelocity.magnitude < .1f && cRigidbody2D.velocity.magnitude > .1f;
            var velocity = !playerStuck ? cRigidbody2D.velocity : Vector2.zero;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = iMovement.ReadValue<Vector2>();
            var movementVector = new Vector2(movementInput.x, movementInput.y > 0f
                ? movementInput.y * (onGround ? 5f : .25f)
                : movementInput.y * .5f);
            movementPush = playerStuck && movementInput.magnitude > 0f
                ? movementPush + Time.fixedDeltaTime
                : Mathf.Max(movementPush - speed * Time.fixedDeltaTime, 1f);
            movementVector *= movementPush;
            cRigidbody2D.AddForce(movementVector * movementForce);

            // Apply boost movement
            var boostInput = iBoost.IsPressed() && cPlayerController.boostCooldown == 0f;
            if (boostInput)
            {
                cPlayerController.boostCharge = Mathf.Min(cPlayerController.boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (lastBoostInput)
            {
                var boost = Mathf.Pow(cPlayerController.boostCharge, 2f) + 2f;
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

            // Save the current position and speed for future reference and update ground status
            lastPosition = transform.position;
            lastSpeed = cRigidbody2D.velocity.magnitude;
            onGround = onGroundFrames > 0;
            onGroundFrames = Mathf.Max(onGroundFrames - 1, 0);
        }
    }
}
