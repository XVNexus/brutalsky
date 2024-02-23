using UnityEngine;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Variables
        public float movementForce = 25f;
        private float movementPush;
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
            if (!other.gameObject.CompareTag("Player")) return;

            // Get collision info
            var impactSpeed = lastSpeed;
            
            // Reduce velocity based on collision force
            var velocityFactor = Mathf.Min(10f / impactSpeed, 1f) / 2f;
            cRigidbody2D.velocity *= velocityFactor;
        }

        // Updates
        private void FixedUpdate()
        {
            // Get movement data
            var position = (Vector2)transform.position;
            var derivedVelocity = (position - lastPosition) / Time.fixedDeltaTime;
            //// This works because glue causes rapid vibration,
            //// resulting in actual velocity being much higher than derived velocity.
            var playerStuck = derivedVelocity.magnitude < .1f && cRigidbody2D.velocity.magnitude > .1f;
            var velocity = !playerStuck ? cRigidbody2D.velocity : Vector2.zero;
            var speed = velocity.magnitude;

            // Apply directional movement
            var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            movementPush = playerStuck && movementInput.magnitude > 0f
                ? movementPush + Time.fixedDeltaTime
                : Mathf.Max(movementPush - speed * Time.fixedDeltaTime, 1f);
            movementInput *= movementPush;
            cRigidbody2D.AddForce(movementInput * movementForce);

            // Apply boost movement
            var boostInput = Input.GetButton("Jump") && cPlayerController.boostCooldown == 0f;
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

            // Save the current position and speed for future reference
            lastPosition = transform.position;
            lastSpeed = cRigidbody2D.velocity.magnitude;
        }
    }
}
