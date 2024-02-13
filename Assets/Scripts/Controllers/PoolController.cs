using UnityEngine;
using Utils;

public class PoolController : MonoBehaviour
{
    // Settings
    public float buoyancy;
    public float viscosity;

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
        buoyancyForce = new Vector2(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle)) * buoyancy;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Get collision info
        var rigidbody = other.attachedRigidbody;
        var bounds = other.bounds;

        // Calculate fluid force
        var objRadius = MathfExt.WeightedMean(
            bounds.extents.x, Mathf.Abs(Mathf.Cos(surfaceAngle)),
            bounds.extents.y, Mathf.Abs(Mathf.Sin(surfaceAngle)));
        var relativePosition = transform.InverseTransformPoint(bounds.center);
        var objDistance = relativePosition.y * transform.localScale.y - surfaceDistance;
        var submersionFactor = Mathf.Max(MathfExt.InverseLerp(objRadius, -objRadius, objDistance), 0f);

        // Apply fluid force
        var velocity = rigidbody.velocity;
        rigidbody.velocity = velocity - velocity * (viscosity * Time.fixedDeltaTime);
        var angularVelocity = rigidbody.angularVelocity;
        rigidbody.angularVelocity = angularVelocity - angularVelocity * (viscosity * Time.fixedDeltaTime);
        rigidbody.AddForceAtPosition(buoyancyForce * (submersionFactor * buoyancy), rigidbody.worldCenterOfMass);
    }
}
