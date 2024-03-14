using Brutalsky.Object;
using UnityEngine;

namespace Brutalsky.Joint
{
    public class BsJointSlider : BsJoint
    {
        public BsJointConfig Angle { get; set; }
        public BsJointMotor Motor { get; set; }
        public BsJointLimits Limits { get; set; }

        public BsJointSlider(string id, BsTransform transform, string targetShapeId, string mountShapeId,
            bool collision, BsJointStrength strength, BsJointConfig angle, BsJointMotor motor, BsJointLimits limits)
            : base(BsJointType.Slider, id, transform, targetShapeId, mountShapeId, collision, strength)
        {
            Angle = angle;
            Motor = motor;
            Limits = limits;
        }

        public override void ApplyConfigToInstance(AnchoredJoint2D jointComponent)
        {
            var sliderJointComponent = (SliderJoint2D)jointComponent;
            sliderJointComponent.angle = Angle.Value;
            sliderJointComponent.autoConfigureAngle = Angle.Auto;
            if (Motor.Use)
            {
                sliderJointComponent.useMotor = true;
                var sliderJointMotor = sliderJointComponent.motor;
                sliderJointMotor.motorSpeed = Motor.Speed;
                sliderJointMotor.maxMotorTorque = Motor.MaxForce;
                sliderJointComponent.motor = sliderJointMotor;
            }
            if (Limits.Use)
            {
                sliderJointComponent.useLimits = true;
                var sliderJointLimits = sliderJointComponent.limits;
                sliderJointLimits.min = Limits.Min;
                sliderJointLimits.max = Limits.Max;
                sliderJointComponent.limits = sliderJointLimits;
            }
        }
    }
}
