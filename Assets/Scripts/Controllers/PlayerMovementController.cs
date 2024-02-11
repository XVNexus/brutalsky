using UnityEngine;

namespace Controllers
{
    public class PlayerMovementController : MonoBehaviour
    {
        /// <summary>
        /// The arrow key speed
        /// </summary>
        public float moveForce = 25f;

        private Rigidbody2D cRigidbody2D;
        public float boostCharge = 0f;
        public float boostCooldown = 0f;
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
