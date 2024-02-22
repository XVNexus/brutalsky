using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointWheel : BsJoint
    {
        public BsJointDamping suspensionDamping { get; set; }
        public float suspensionAngle { get; set; }
        public BsJointMotor motor { get; set; }

        public BsJointWheel(string id, string targetShapeId, string mountShapeId, bool collision,
            BsJointStrength strength, BsJointDamping suspensionDamping, float suspensionAngle, BsJointMotor motor)
            : base(BsJointType.Wheel, id, targetShapeId, mountShapeId, collision, strength)
        {
            this.suspensionDamping = suspensionDamping;
            this.suspensionAngle = suspensionAngle;
            this.motor = motor;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var wheelJointComponent = (WheelJoint2D)jointComponent;
            var wheelJointSuspension = wheelJointComponent.suspension;
            wheelJointSuspension.dampingRatio = suspensionDamping.ratio;
            wheelJointSuspension.frequency = suspensionDamping.frequency;
            wheelJointSuspension.angle = suspensionAngle;
            wheelJointComponent.suspension = wheelJointSuspension;
            if (motor.use)
            {
                wheelJointComponent.useMotor = true;
                var wheelJointMotor = wheelJointComponent.motor;
                wheelJointMotor.motorSpeed = motor.speed;
                wheelJointMotor.maxMotorTorque = motor.maxForce;
                wheelJointComponent.motor = wheelJointMotor;
            }
        }
    }
}
