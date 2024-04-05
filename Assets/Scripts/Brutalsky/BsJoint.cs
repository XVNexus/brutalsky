using Brutalsky.Base;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using JointLimits = Utils.Joint.JointLimits;
using JointMotor = Utils.Joint.JointMotor;

namespace Brutalsky
{
    public class BsJoint : BsObject
    {
        public JointType JointType { get; private set; }
        public string TargetShapeId { get; set; }
        public string MountShapeId { get; set; }

        public bool Collision { get; set; } // Universal
        public JointStrength Strength { get; set; } // Universal
        public JointConfig Distance { get; set; } // Distance, Spring
        public bool MaxDistanceOnly { get; set; } // Distance
        public JointDamping Damping { get; set; } // Fixed, Spring, Wheel
        public JointMotor Motor { get; set; } // Hinge, Slider, Wheel
        public JointLimits Limits { get; set; } // Hinge, Slider
        public JointConfig Angle { get; set; } // Slider, Wheel

        public BsJoint(string id, ObjectTransform transform, string targetShapeId,
            string mountShapeId, bool collision, JointStrength strength) : base(id, transform)
        {
            TargetShapeId = targetShapeId;
            MountShapeId = mountShapeId;
            Collision = collision;
            Strength = strength;
        }

        public BsJoint FixedJoint(JointDamping damping)
        {
            JointType = JointType.Fixed;
            Damping = damping;
            return this;
        }

        public BsJoint DistanceJoint(JointConfig distance, bool maxDistanceOnly)
        {
            JointType = JointType.Distance;
            Distance = distance;
            MaxDistanceOnly = maxDistanceOnly;
            return this;
        }

        public BsJoint SpringJoint(JointConfig distance, JointDamping damping)
        {
            JointType = JointType.Spring;
            Distance = distance;
            Damping = damping;
            return this;
        }

        public BsJoint HingeJoint(JointMotor motor, JointLimits limits)
        {
            JointType = JointType.Hinge;
            Motor = motor;
            Limits = limits;
            return this;
        }

        public BsJoint SliderJoint(JointConfig angle, JointMotor motor, JointLimits limits)
        {
            JointType = JointType.Slider;
            Angle = angle;
            Motor = motor;
            Limits = limits;
            return this;
        }

        public BsJoint WheelJoint(JointDamping damping, JointConfig angle, JointMotor motor)
        {
            JointType = JointType.Wheel;
            Damping = damping;
            Angle = angle;
            Motor = motor;
            return this;
        }

        public void ApplyConfigToInstance(AnchoredJoint2D jointComponent, [CanBeNull] BsShape mountShape)
        {
            // Apply universal joint config
            jointComponent.anchor = Transform.Position;
            jointComponent.enableCollision = Collision;
            jointComponent.breakForce = Strength.BreakForce;
            jointComponent.breakTorque = Strength.BreakTorque;

            // Apply specific joint config
            switch (JointType)
            {
                case JointType.Fixed:
                    var fixedJointComponent = (FixedJoint2D)jointComponent;
                    fixedJointComponent.dampingRatio = Damping.Ratio;
                    fixedJointComponent.frequency = Damping.Frequency;
                    break;

                case JointType.Distance:
                    var distanceJointComponent = (DistanceJoint2D)jointComponent;
                    distanceJointComponent.distance = Distance.Value;
                    distanceJointComponent.autoConfigureDistance = Distance.Auto;
                    distanceJointComponent.maxDistanceOnly = MaxDistanceOnly;
                    break;

                case JointType.Spring:
                    var springJointComponent = (SpringJoint2D)jointComponent;
                    springJointComponent.distance = Distance.Value;
                    springJointComponent.autoConfigureDistance = Distance.Auto;
                    springJointComponent.dampingRatio = Damping.Ratio;
                    springJointComponent.frequency = Damping.Frequency;
                    break;

                case JointType.Hinge:
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
                    break;

                case JointType.Slider:
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
                    break;

                case JointType.Wheel:
                    var wheelJointComponent = (WheelJoint2D)jointComponent;
                    var wheelJointSuspension = wheelJointComponent.suspension;
                    wheelJointSuspension.dampingRatio = Damping.Ratio;
                    wheelJointSuspension.frequency = Damping.Frequency;
                    wheelJointSuspension.angle = Angle.Value;
                    wheelJointComponent.suspension = wheelJointSuspension;
                    if (Motor.Use)
                    {
                        wheelJointComponent.useMotor = true;
                        var wheelJointMotor = wheelJointComponent.motor;
                        wheelJointMotor.motorSpeed = Motor.Speed;
                        wheelJointMotor.maxMotorTorque = Motor.MaxForce;
                        wheelJointComponent.motor = wheelJointMotor;
                    }
                    break;

                default:
                    throw Errors.InvalidJointType(JointType);
            }

            // Set up connected rigidbody
            if (mountShape == null) return;
            if (mountShape.InstanceObject == null)
            {
                throw Errors.MountShapeUnbuilt(this);
            }
            jointComponent.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
            jointComponent.autoConfigureConnectedAnchor = true;
        }
    }
}
