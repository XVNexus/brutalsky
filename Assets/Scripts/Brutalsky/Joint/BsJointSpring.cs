using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointSpring : BsJoint
    {
        public BsJointConfig Distance { get; set; }
        public BsJointDamping Damping { get; set; }

        public BsJointSpring(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointConfig distance, BsJointDamping damping)
            : base(BsJointType.Spring, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            Distance = distance;
            Damping = damping;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var springJointComponent = (SpringJoint2D)jointComponent;
            springJointComponent.distance = Distance.Value;
            springJointComponent.autoConfigureDistance = Distance.Auto;
            springJointComponent.dampingRatio = Damping.Ratio;
            springJointComponent.frequency = Damping.Frequency;
        }
    }
}
