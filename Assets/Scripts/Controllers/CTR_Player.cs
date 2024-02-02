using UnityEngine;

namespace Controllers
{
    public class CTR_Player : MonoBehaviour
    {
        public float moveForce = 20f;

        private Rigidbody2D _cRigidbody2D;
    
        private void Start()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var iMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var forceMultiplier = Mathf.Clamp(1f / _cRigidbody2D.velocity.magnitude, .5f, 5f);
            _cRigidbody2D.AddForce(iMovement * (moveForce * forceMultiplier));
        }
    }
}
