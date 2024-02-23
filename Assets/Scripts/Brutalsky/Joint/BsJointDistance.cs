using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointDistance : BsJoint
    {
        public BsJointConfig distance { get; set; }
        public bool maxDistanceOnly { get; set; }

        public BsJointDistance(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointConfig distance, bool maxDistanceOnly)
            : base(BsJointType.Distance, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            this.distance = distance;
            this.maxDistanceOnly = maxDistanceOnly;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var distanceJointComponent = (DistanceJoint2D)jointComponent;
            distanceJointComponent.distance = distance.value;
            distanceJointComponent.autoConfigureDistance = distance.autoConfig;
            distanceJointComponent.maxDistanceOnly = maxDistanceOnly;
        }
    }
}
