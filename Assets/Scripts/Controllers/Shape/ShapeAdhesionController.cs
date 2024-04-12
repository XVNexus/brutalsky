using Brutalsky;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;

namespace Controllers.Shape
{
    public class ShapeAdhesionController : SubControllerBase<BsShape>
    {
        // Controller metadata
        public override string Id => "adhesion";
        public override bool IsUnused => Master.Object.Material.Adhesion == 0f || !Master.Object.Simulated;

        // Component references
        private Rigidbody2D _cRigidbody2D;

        // Init functions
        protected override void OnInit()
        {
            _cRigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Event functions
        private void OnCollisionStay2D(Collision2D other)
        {
            // Get collision info
            var otherRigidbody = other.rigidbody;
            var thisDynamic = Master.Object.Material.Dynamic;
            var otherDynamic = other.gameObject.CompareTag(Tags.PlayerName)
                || (other.gameObject.CompareTag(Tags.ShapeName)
                    && other.gameObject.GetComponent<ShapeController>().Object.Material.Dynamic);

            // Apply adhesion force
            var contact = other.GetContact(0);
            var adhesionForce = Master.Object.Material.Adhesion * contact.normal;
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
