using UnityEngine;

namespace Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        // Settings
        public float moveForce = 25f;

        // Variables
        public float boostCharge;
        public float boostCooldown;
        private bool lastBoostInput;

        // References
        private Rigidbody2D cRigidbody2D;
    
        // Events
        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Updates
        private void FixedUpdate()
        {
            // Apply directional movement
            var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            cRigidbody2D.AddForce(movementInput * moveForce);

            // Apply boost movement
            var boostInput = Input.GetButton("Jump") && boostCooldown == 0f;
            if (boostInput)
            {
                boostCharge = Mathf.Min(boostCharge + Time.fixedDeltaTime, 3f);
            }
            else if (lastBoostInput)
            {
                var boost = Mathf.Pow(boostCharge, 2f) + 2f;
                boostCooldown = boost;
                boostCharge = 0f;
                cRigidbody2D.velocity *= boost;
            }
            if (boostCooldown > 0f)
            {
                boostCooldown = Mathf.Max(boostCooldown - Time.fixedDeltaTime, 0f);
            }
            lastBoostInput = boostInput;
        }
    }
}
