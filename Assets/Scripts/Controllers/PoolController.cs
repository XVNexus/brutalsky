using Brutalsky;
using UnityEngine;
using Utils;

namespace Controllers
{
    public class PoolController : MonoBehaviour
    {
        // Source
        public BsPool bsObject;

        // Variables
        private Vector2 buoyancyForce;
        private float surfaceAngle;
        private float surfacePosition;

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
            surfaceAngle = (poolTransform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            surfacePosition = bsObject.size.y * .5f;
            buoyancyForce = new Vector2(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle)) * bsObject.chemical.buoyancy;
            var poolPosition = poolTransform.position;
            poolPosition -= (Vector3)MathfExt.RotateVector(new Vector2(lineWidth * .25f, 0f), surfaceAngle);
            poolTransform.position = poolPosition;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Get collision info
            var otherRigidbody = other.attachedRigidbody;
            var bounds = other.bounds;

            // Calculate fluid force
            var objRadius = MathfExt.WeightedMean(
                bounds.extents.x, Mathf.Abs(Mathf.Cos(surfaceAngle)),
                bounds.extents.y, Mathf.Abs(Mathf.Sin(surfaceAngle)));
            var poolTransform = transform;
            var relativePosition = poolTransform.InverseTransformPoint(bounds.center);
            var objDistance = relativePosition.y * poolTransform.localScale.y - surfacePosition;
            var submersionFactor = Mathf.Max(MathfExt.InverseLerp(objRadius, -objRadius, objDistance), 0f);

            // Apply fluid force
            var velocity = otherRigidbody.velocity;
            otherRigidbody.velocity = velocity - velocity * (bsObject.chemical.viscosity * Time.fixedDeltaTime);
            var angularVelocity = otherRigidbody.angularVelocity;
            otherRigidbody.angularVelocity = angularVelocity - angularVelocity * (bsObject.chemical.viscosity * Time.fixedDeltaTime);
            otherRigidbody.AddForceAtPosition(buoyancyForce * submersionFactor, otherRigidbody.worldCenterOfMass);

            // Apply damage to player
            var damage = bsObject.chemical.damage;
            if (!other.gameObject.CompareTag("Player") || damage == 0f) return;
            var playerController = other.gameObject.GetComponent<PlayerController>();
            if (damage > 0f)
            {
                playerController.Hurt(damage * Time.fixedDeltaTime);
            }
            else
            {
                playerController.Heal(-damage * Time.fixedDeltaTime);
            }
        }
    }
}
