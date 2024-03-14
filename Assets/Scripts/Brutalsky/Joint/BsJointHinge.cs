using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointHinge : BsJoint
    {
        public BsJointMotor Motor { get; set; }
        public BsJointLimits Limits { get; set; }

        public BsJointHinge(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointMotor motor, BsJointLimits limits)
            : base(BsJointType.Hinge, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            Motor = motor;
            Limits = limits;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var hingeJointComponent = (HingeJoint2D)jointComponent;
            if (Motor.Use)
            {
                hingeJointComponent.useMotor = true;
                var hingeJointMotor = hingeJointComponent.motor;
                hingeJointMotor.motorSpeed = Motor.Speed;
                hingeJointMotor.maxMotorTorque = Motor.MaxForce;
                hingeJointComponent.motor = hingeJointMotor;
            }
            if (Limits.Use)
            {
                hingeJointComponent.useLimits = true;
                var hingeJointLimits = hingeJointComponent.limits;
                hingeJointLimits.min = Limits.Min;
                hingeJointLimits.max = Limits.Max;
                hingeJointComponent.limits = hingeJointLimits;
            }
        }
    }
}
