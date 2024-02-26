using Brutalsky;
using UnityEngine;

namespace Controllers
{
    public class ShapeController : MonoBehaviour
    {
        // Source
        public BsShape bsObject;

        // References
        private Rigidbody2D cRigidbody2D;

        // Events
        private void OnEnable()
        {
            cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Apply damage to player
            var damage = bsObject.material.damage;
            if (!other.gameObject.CompareTag("Player") || damage == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Hurt(damage);
            }
            else
            {
                playerController.Heal(-damage);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (bsObject.material.adhesion == 0f) return;

            // Get collision info
            var otherRigidbody = other.rigidbody;
            var thisDynamic = bsObject.material.dynamic;
            var otherDynamic = other.gameObject.CompareTag("Player")
                || (other.gameObject.CompareTag("Shape")
                    && other.gameObject.GetComponent<ShapeController>().bsObject.material.dynamic);

            // Apply adhesion force
            var contact = other.GetContact(0);
            var adhesionForce = bsObject.material.adhesion * contact.normal;
            if (thisDynamic)
            {
                cRigidbody2D.AddForceAtPosition(-adhesionForce, contact.point, ForceMode2D.Impulse);
            }
            if (otherDynamic)
            {
                otherRigidbody.AddForceAtPosition(adhesionForce, contact.point, ForceMode2D.Impulse);
            }
        }
    }
}
