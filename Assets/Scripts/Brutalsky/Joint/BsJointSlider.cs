using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointSlider : BsJoint
    {
        public BsJointConfig angle { get; set; }
        public BsJointMotor motor { get; set; }
        public BsJointLimits limits { get; set; }

        public BsJointSlider(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointConfig angle, BsJointMotor motor, BsJointLimits limits)
            : base(BsJointType.Slider, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            this.angle = angle;
            this.motor = motor;
            this.limits = limits;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var sliderJointComponent = (SliderJoint2D)jointComponent;
            sliderJointComponent.angle = angle.value;
            sliderJointComponent.autoConfigureAngle = angle.autoConfig;
            if (motor.use)
            {
                sliderJointComponent.useMotor = true;
                var sliderJointMotor = sliderJointComponent.motor;
                sliderJointMotor.motorSpeed = motor.speed;
                sliderJointMotor.maxMotorTorque = motor.maxForce;
                sliderJointComponent.motor = sliderJointMotor;
            }
            if (limits.use)
            {
                sliderJointComponent.useLimits = true;
                var sliderJointLimits = sliderJointComponent.limits;
                sliderJointLimits.min = limits.min;
                sliderJointLimits.max = limits.max;
                sliderJointComponent.limits = sliderJointLimits;
            }
        }
    }
}
