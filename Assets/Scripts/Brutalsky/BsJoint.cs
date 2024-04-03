using Brutalsky.Joint;
using Brutalsky.Object;
using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Utils.Constants;

namespace Brutalsky
{
    public abstract class BsJoint : BsObject
    {
        public BsJointType JointType { get; }

        public string TargetShapeId { get; set; }
        public string MountShapeId { get; set; }

        public bool Collision { get; set; }
        public BsJointStrength Strength { get; set; }

        public BsJoint(BsJointType jointType, string id, BsTransform transform, string targetShapeId,
            string mountShapeId, bool collision, BsJointStrength strength) : base(id, transform)
        {
            JointType = jointType;
            TargetShapeId = targetShapeId;
            MountShapeId = mountShapeId;
            Collision = collision;
            Strength = strength;
        }

        public void ApplyBaseConfigToInstance(AnchoredJoint2D jointComponent, [CanBeNull] BsShape mountShape)
        {
            // Apply joint config
            jointComponent.anchor = Transform.Position;
            jointComponent.enableCollision = Collision;
            jointComponent.breakForce = Strength.BreakForce;
            jointComponent.breakTorque = Strength.BreakTorque;

            // Set up connected rigidbody
            if (mountShape == null) return;
            if (mountShape.InstanceObject == null)
            {
                throw Errors.MountShapeUnbuilt(this);
            }
            jointComponent.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
            jointComponent.autoConfigureConnectedAnchor = true;
        }

        public abstract void ApplyConfigToInstance(AnchoredJoint2D jointComponent);
    }
}
