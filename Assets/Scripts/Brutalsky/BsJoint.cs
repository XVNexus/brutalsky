using System;
using Brutalsky.Joint;
using Brutalsky.Object;
using JetBrains.Annotations;
using UnityEngine;

namespace Brutalsky
{
    public abstract class BsJoint : BsObject
    {
        public BsJointType jointType { get; }

        public string targetShapeId { get; set; }
        public string mountShapeId { get; set; }

        public bool collision { get; set; }
        public BsJointStrength strength { get; set; }

        public BsJoint(BsJointType jointType, string id, BsTransform transform, string targetShapeId, string mountShapeId, bool collision,
            BsJointStrength strength) : base(id, transform)
        {
            this.jointType = jointType;

            this.targetShapeId = targetShapeId;
            this.mountShapeId = mountShapeId;

            this.collision = collision;
            this.strength = strength;
        }

        public void ApplyBaseConfigToInstance(AnchoredJoint2D jointComponent, [CanBeNull] BsShape mountShape)
        {
            // Apply joint config
            jointComponent.anchor = transform.position;
            jointComponent.enableCollision = collision;
            jointComponent.breakForce = strength.breakForce;
            jointComponent.breakTorque = strength.breakTorque;

            // Set up connected rigidbody
            if (mountShape == null) return;
            if (mountShape.instanceObject == null)
            {
                throw new ArgumentNullException(nameof(mountShape), "Joints cannot be built while attached shapes are inactive");
            }
            jointComponent.connectedBody = mountShape.instanceObject.GetComponent<Rigidbody2D>();
            jointComponent.autoConfigureConnectedAnchor = true;
        }

        public abstract void ApplyConfigToInstance(AnchoredJoint2D jointComponent);
    }
}
