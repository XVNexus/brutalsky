using UnityEngine;

namespace Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        /// <summary>
        /// The arrow key speed
        /// </summary>
        public float moveForce = 25f;
        /// <summary>
        /// The boost key speed
        /// </summary>
        public float boostForce = 25f;

        private Rigidbody2D cRigidbody2D;

        private float boostCharge = 0f;
        private float boostCooldown = 0f;
        private bool lastBoostInput = false;
    
        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            cRigidbody2D.AddForce(movementInput * moveForce);
            
            var boostInput = Input.GetButton("Jump") && boostCooldown == 0f;
            if (boostInput)
            {
                boostCharge += Time.fixedDeltaTime;
            }
            else if (lastBoostInput)
            {
                var boost = boostCharge * boostForce + 1f;
                // boostCooldown = 3f + boostCharge * 2f;
                boostCharge = 0f;
                cRigidbody2D.AddForce(boost * cRigidbody2D.velocity.normalized, ForceMode2D.Impulse);
            }
            if (boostCooldown > 0f)
            {
                boostCooldown = Mathf.Max(boostCooldown - Time.fixedDeltaTime, 0f);
            }
            lastBoostInput = boostInput;
        }
    }
}
