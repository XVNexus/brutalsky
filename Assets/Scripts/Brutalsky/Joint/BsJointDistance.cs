using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointDistance : BsJoint
    {
        public BsJointConfig Distance { get; set; }
        public bool MaxDistanceOnly { get; set; }

        public BsJointDistance(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointConfig distance, bool maxDistanceOnly)
            : base(BsJointType.Distance, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            Distance = distance;
            MaxDistanceOnly = maxDistanceOnly;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var distanceJointComponent = (DistanceJoint2D)jointComponent;
            distanceJointComponent.distance = Distance.Value;
            distanceJointComponent.autoConfigureDistance = Distance.Auto;
            distanceJointComponent.maxDistanceOnly = MaxDistanceOnly;
        }
    }
}
