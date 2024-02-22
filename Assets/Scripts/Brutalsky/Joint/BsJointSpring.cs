using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointSpring : BsJoint
    {
        public BsJointConfig distance { get; set; }
        public BsJointDamping damping { get; set; }

        public BsJointSpring(string id, string targetShapeId, string mountShapeId, bool collision,
            BsJointStrength strength, BsJointConfig distance, BsJointDamping damping)
            : base(BsJointType.Spring, id, targetShapeId, mountShapeId, collision, strength)
        {
            this.distance = distance;
            this.damping = damping;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var springJointComponent = (SpringJoint2D)jointComponent;
            springJointComponent.distance = distance.value;
            springJointComponent.autoConfigureDistance = distance.autoConfig;
            springJointComponent.dampingRatio = damping.ratio;
            springJointComponent.frequency = damping.frequency;
        }
    }
}
