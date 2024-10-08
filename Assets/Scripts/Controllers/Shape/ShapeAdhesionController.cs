using Controllers.Base;
using Data.Object;
using UnityEngine;
using Utils;

namespace Controllers.Shape
{
    public class ShapeAdhesionController : SubControllerBase<BsShape>
    {
        // Controller metadata
        public override string Id => "adhesion";
        public override bool IsUnused => Master.Object.Adhesion == 0f;

        // External references
        private Rigidbody2D _cRigidbody2D;

        // Init functions
        protected override void OnInit()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Event functions
        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(Tags.PlayerTag)) return;

            // Allow players to unstick from low to moderate adhesion
            var playerRigidbody = other.rigidbody;
            var playerDirection = -playerRigidbody.velocity.normalized;
            var adhesionForce = Master.Object.Adhesion * playerDirection;
            playerRigidbody.position += playerDirection * .1f;
            playerRigidbody.AddForce(adhesionForce, ForceMode2D.Impulse);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            // Get collision info
            var otherRigidbody = other.rigidbody;
            var thisDynamic = Master.Object.Dynamic;
            var otherDynamic = other.gameObject.CompareTag(Tags.PlayerTag)
                || (other.gameObject.CompareTag(Tags.ShapeTag)
                    && other.gameObject.GetComponent<ShapeController>().Object.Dynamic);

            // Apply adhesion force
            var contact = other.GetContact(0);
            var adhesionForce = Master.Object.Adhesion * contact.normal;
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
