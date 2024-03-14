using Brutalsky;
using UnityEngine;

namespace Controllers
{
    public class ShapeController : MonoBehaviour
    {
        // Constants
        public const string Tag = "Shape";

        // Source
        public BsShape BsObject;

        // References
        private Rigidbody2D _cRigidbody2D;

        // Events
        private void OnCollisionEnter2D(Collision2D other)
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();

            // Apply damage to player
            var damage = BsObject.Material.Damage;
            if (!other.gameObject.CompareTag(PlayerController.Tag) || damage == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Heal(damage);
            }
            else
            {
                playerController.Hurt(-damage);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (BsObject.Material.Adhesion == 0f) return;

            // Get collision info
            var otherRigidbody = other.rigidbody;
            var thisDynamic = BsObject.Material.Dynamic;
            var otherDynamic = other.gameObject.CompareTag(PlayerController.Tag)
                || (other.gameObject.CompareTag(Tag)
                    && other.gameObject.GetComponent<ShapeController>().BsObject.Material.Dynamic);

            // Apply adhesion force
            var contact = other.GetContact(0);
            var adhesionForce = BsObject.Material.Adhesion * contact.normal;
            if (thisDynamic)
            {
                _cRigidbody2D.AddForceAtPosition(-adhesionForce, contact.point, ForceMode2D.Impulse);
            }
            if (otherDynamic)
            {
                otherRigidbody.AddForceAtPosition(adhesionForce, contact.point, ForceMode2D.Impulse);
            }
        }
    }
}
