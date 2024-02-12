using System.Linq;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PlayerCameraController : MonoBehaviour
    {
        public float lastSpeed;

        private Rigidbody2D cRigidbody2D;

        private void Start()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            // Save the current speed for future reference
            lastSpeed = cRigidbody2D.velocity.magnitude;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Get collision info
            var impactForce = other.contacts.Sum(contact => contact.normalImpulse);
            var impactDirection = ((Vector2)transform.position - other.contacts[0].point).normalized;

            // Apply camera shake
            var shakeForce = Mathf.Min(MathfExt.TMP(impactForce, 25f, .5f, 1.5f) * .02f, 5f);
            if (other.gameObject.CompareTag("Player"))
            {
                EventSystem.current.TriggerCameraShake(Vector2.zero, shakeForce);
            }
            else
            {
                EventSystem.current.TriggerCameraShake(shakeForce * impactDirection, 0f);
            }
        }
    }
}
