using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointFixed : BsJoint
    {
        public BsJointDamping damping { get; set; }

        public BsJointFixed(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointDamping damping)
            : base(BsJointType.Fixed, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            this.damping = damping;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var fixedJointComponent = (FixedJoint2D)jointComponent;
            fixedJointComponent.dampingRatio = damping.ratio;
            fixedJointComponent.frequency = damping.frequency;
        }
    }
}