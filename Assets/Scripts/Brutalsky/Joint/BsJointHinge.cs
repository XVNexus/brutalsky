using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointHinge : BsJoint
    {
        public BsJointMotor motor { get; set; }
        public BsJointLimits limits { get; set; }

        public BsJointHinge(string id, string targetShapeId, string mountShapeId, bool collision,
            BsJointStrength strength, BsJointMotor motor, BsJointLimits limits)
            : base(BsJointType.Hinge, id, targetShapeId, mountShapeId, collision, strength)
        {
            this.motor = motor;
            this.limits = limits;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var hingeJointComponent = (HingeJoint2D)jointComponent;
            if (motor.use)
            {
                hingeJointComponent.useMotor = true;
                var hingeJointMotor = hingeJointComponent.motor;
                hingeJointMotor.motorSpeed = motor.speed;
                hingeJointMotor.maxMotorTorque = motor.maxForce;
                hingeJointComponent.motor = hingeJointMotor;
            }
            if (limits.use)
            {
                hingeJointComponent.useLimits = true;
                var hingeJointLimits = hingeJointComponent.limits;
                hingeJointLimits.min = limits.min;
                hingeJointLimits.max = limits.max;
                hingeJointComponent.limits = hingeJointLimits;
            }
        }
    }
}
