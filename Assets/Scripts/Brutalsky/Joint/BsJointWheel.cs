using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointWheel : BsJoint
    {
        public BsJointDamping SuspensionDamping { get; set; }
        public float SuspensionAngle { get; set; }
        public BsJointMotor Motor { get; set; }

        public BsJointWheel(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointDamping suspensionDamping, float suspensionAngle, BsJointMotor motor)
            : base(BsJointType.Wheel, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            SuspensionDamping = suspensionDamping;
            SuspensionAngle = suspensionAngle;
            Motor = motor;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var wheelJointComponent = (WheelJoint2D)jointComponent;
            var wheelJointSuspension = wheelJointComponent.suspension;
            wheelJointSuspension.dampingRatio = SuspensionDamping.Ratio;
            wheelJointSuspension.frequency = SuspensionDamping.Frequency;
            wheelJointSuspension.angle = SuspensionAngle;
            wheelJointComponent.suspension = wheelJointSuspension;
            if (Motor.Use)
            {
                wheelJointComponent.useMotor = true;
                var wheelJointMotor = wheelJointComponent.motor;
                wheelJointMotor.motorSpeed = Motor.Speed;
                wheelJointMotor.maxMotorTorque = Motor.MaxForce;
                wheelJointComponent.motor = wheelJointMotor;
            }
        }
    }
}
