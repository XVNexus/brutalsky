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
        public float wavePointDensity = 2f;
        private Vector2 buoyancyForce;
        private float surfaceAngle;
        private float surfacePosition;
        private float lineWidth;

        // References
        public LineRenderer cLineRenderer;
        private SpriteRenderer cSpriteRenderer;

        // Events
        private void Start()
        {
            cSpriteRenderer = GetComponent<SpriteRenderer>();

            // Set wave color to match the pool color
            var color = bsObject.color.tint;
            cLineRenderer.startColor = color;
            cLineRenderer.endColor = color;
            cLineRenderer.sortingOrder = cSpriteRenderer.sortingOrder;

            // Set up fluid properties
            lineWidth = cLineRenderer.widthMultiplier;
            var poolTransform = transform;
            surfaceAngle = (poolTransform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            surfacePosition = bsObject.size.y * .5f;
            buoyancyForce = new Vector2(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle)) * bsObject.chemical.buoyancy;

            // Set up wave renderer
            cLineRenderer.positionCount = Mathf.RoundToInt(bsObject.size.x * wavePointDensity) + 3;
            var posCount = cLineRenderer.positionCount;
            var surfaceOffset = lineWidth * 2f / bsObject.size.y;
            var edgeOffset = lineWidth * .5f / bsObject.size.x;
            var wavePointInterval = 1f / wavePointDensity / bsObject.size.x;
            Debug.Log($"Size: {bsObject.size} / Surface Offset: {surfaceOffset * bsObject.size.y} / Edge Offset: {edgeOffset * bsObject.size.x} / Point Interval: {wavePointInterval * bsObject.size.x}");
            cLineRenderer.SetPosition(0, new Vector2(.5f - edgeOffset, -surfaceOffset));
            cLineRenderer.SetPosition(1, new Vector2(.5f - edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 2, new Vector2(-.5f + edgeOffset, 0f));
            cLineRenderer.SetPosition(posCount - 1, new Vector2(-.5f + edgeOffset, -surfaceOffset));
            for (var i = 2; i < cLineRenderer.positionCount - 2; i++)
            {
                cLineRenderer.SetPosition(i, new Vector2(-.5f + wavePointInterval * (i - 1), 0f));
            }
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
            var objDistance = relativePosition.y * transform.localScale.y - surfacePosition;
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
