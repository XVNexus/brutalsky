using Brutalsky.Object;
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
        private float surfaceDistance;

        // Events
        private void Start()
        {
            // Set up fluid properties
            var poolTransform = transform;
            surfaceAngle = (poolTransform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            surfaceDistance = poolTransform.localScale.y / 2f;
            buoyancyForce = new Vector2(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle)) * bsObject.chemical.buoyancy;
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
            var relativePosition = transform.InverseTransformPoint(bounds.center);
            var objDistance = relativePosition.y * transform.localScale.y - surfaceDistance;
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
