using Brutalsky;
using UnityEngine;
using Utils.Ext;

namespace Controllers
{
    public class PoolController : MonoBehaviour
    {
        // Constants
        public const string Tag = "Pool";

        // Source
        public BsPool BsObject;

        // Variables
        private Vector2 _buoyancyForce;
        private float _surfaceAngle;
        private float _surfacePosition;

        // References
        public LineRenderer cLineRenderer;

        // Events
        private void Start()
        {
            // Set up fluid properties
            var lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            var poolScale = poolTransform.localScale;
            poolScale.y -= lineWidth * .5f;
            poolTransform.localScale = poolScale;
            _surfaceAngle = (poolTransform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            _surfacePosition = BsObject.Size.y * .5f;
            _buoyancyForce = new Vector2(Mathf.Cos(_surfaceAngle), Mathf.Sin(_surfaceAngle)) * BsObject.Chemical.Buoyancy;
            var poolPosition = poolTransform.position;
            poolPosition -= (Vector3)MathfExt.RotateVector(new Vector2(lineWidth * .25f, 0f), _surfaceAngle);
            poolTransform.position = poolPosition;
        }

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
            otherRigidbody.velocity = velocity - velocity * (BsObject.Chemical.Viscosity * Time.fixedDeltaTime);
            var angularVelocity = otherRigidbody.angularVelocity;
            otherRigidbody.angularVelocity = angularVelocity - angularVelocity * (BsObject.Chemical.Viscosity * Time.fixedDeltaTime);
            otherRigidbody.AddForceAtPosition(_buoyancyForce * submersionFactor, otherRigidbody.worldCenterOfMass);

            // Apply damage to player
            var damage = BsObject.Chemical.Damage;
            if (!other.gameObject.CompareTag(PlayerController.Tag) || damage == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Heal(damage * Time.fixedDeltaTime);
            }
            else
            {
                playerController.Hurt(-damage * Time.fixedDeltaTime);
            }
        }
    }
}
