using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Ext;

namespace Controllers.Pool
{
    public class PoolFloatController : SubControllerBase<BsPool>
    {
        // Controller metadata
        public override string Id => "float";
        public override bool IsUnused => Master.Object.Buoyancy == 0f && Master.Object.Buoyancy == 0f;

        // Local variables
        private Vector2 _buoyancyForce;
        private float _surfaceAngle;
        private float _surfacePosition;

        // Init functions
        protected override void OnInit()
        {
            // Set up fluid properties
            _surfaceAngle = (transform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            _surfacePosition = Master.Object.Size.y * .5f;
            _buoyancyForce = new Vector2(Mathf.Cos(_surfaceAngle), Mathf.Sin(_surfaceAngle)) * Master.Object.Buoyancy;
        }

        // Event functions
        private void OnTriggerStay2D(Collider2D other)
        {
            // Get collision info
            var otherRigidbody = other.attachedRigidbody;
            var bounds = other.bounds;

            // Calculate fluid force
            var objRadius = MathfExt.WeightedMean(
                bounds.extents.x, Mathf.Abs(Mathf.Cos(_surfaceAngle)),
                bounds.extents.y, Mathf.Abs(Mathf.Sin(_surfaceAngle)));
            var poolTransform = transform;
            var relativePosition = poolTransform.InverseTransformPoint(bounds.center);
            var objDistance = relativePosition.y * poolTransform.localScale.y - _surfacePosition;
            var submersionFactor = Mathf.Max(Mathf.InverseLerp(objRadius, -objRadius, objDistance), 0f);

            // Apply fluid force
            var velocity = otherRigidbody.velocity;
            otherRigidbody.velocity = velocity - velocity * (Master.Object.Viscosity * Time.fixedDeltaTime);
            var angularVelocity = otherRigidbody.angularVelocity;
            otherRigidbody.angularVelocity = angularVelocity - angularVelocity
                * (Master.Object.Viscosity * Time.fixedDeltaTime);
            otherRigidbody.AddForceAtPosition(_buoyancyForce * submersionFactor, otherRigidbody.worldCenterOfMass);
        }
    }
}
