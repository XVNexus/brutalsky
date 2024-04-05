using System.Collections.Generic;
using Brutalsky.Base;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using JointLimits = Utils.Joint.JointLimits;
using JointMotor = Utils.Joint.JointMotor;

namespace Brutalsky
{
    public class BsJoint : BsAddon
    {
        public override string Tag => Tags.Joint;
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

        public BsJoint()
        {
        }

        protected override Component _Init(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            // Get references to linked objects
            var targetShape = (BsShape)parentObject;
            var targetGameObject = targetShape.InstanceObject;
            if (targetGameObject == null)
            {
                throw Errors.TargetShapeUnbuilt(this);
            }

            // Create joint component
            AnchoredJoint2D component = JointType switch
            {
                JointType.Fixed => targetGameObject.AddComponent<FixedJoint2D>(),
                JointType.Distance => targetGameObject.AddComponent<DistanceJoint2D>(),
                JointType.Spring => targetGameObject.AddComponent<SpringJoint2D>(),
                JointType.Hinge => targetGameObject.AddComponent<HingeJoint2D>(),
                JointType.Slider => targetGameObject.AddComponent<SliderJoint2D>(),
                JointType.Wheel => targetGameObject.AddComponent<WheelJoint2D>(),
                _ => throw Errors.InvalidJointType(JointType)
            };

            // Apply universal joint config
            component.anchor = Transform.Position;
            component.enableCollision = Collision;
            component.breakForce = Strength.BreakForce;
            component.breakTorque = Strength.BreakTorque;

            // Apply specific joint config
            switch (JointType)
            {
                case JointType.Fixed:
                    var fixedJointComponent = (FixedJoint2D)component;
                    fixedJointComponent.dampingRatio = Damping.Ratio;
                    fixedJointComponent.frequency = Damping.Frequency;
                    break;

                case JointType.Distance:
                    var distanceJointComponent = (DistanceJoint2D)component;
                    distanceJointComponent.distance = Distance.Value;
                    distanceJointComponent.autoConfigureDistance = Distance.Auto;
                    distanceJointComponent.maxDistanceOnly = MaxDistanceOnly;
                    break;

                case JointType.Spring:
                    var springJointComponent = (SpringJoint2D)component;
                    springJointComponent.distance = Distance.Value;
                    springJointComponent.autoConfigureDistance = Distance.Auto;
                    springJointComponent.dampingRatio = Damping.Ratio;
                    springJointComponent.frequency = Damping.Frequency;
                    break;

                case JointType.Hinge:
                    var hingeJointComponent = (HingeJoint2D)component;
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
                    var sliderJointComponent = (SliderJoint2D)component;
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
                    var wheelJointComponent = (WheelJoint2D)component;
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
            var mountShape = MountShapeId.Length > 0 ? map.GetObject<BsShape>(Tags.Shape, MountShapeId) : null;
            if (mountShape == null) return null;
            if (mountShape.InstanceObject == null)
            {
                throw Errors.MountShapeUnbuilt(this);
            }
            component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
            component.autoConfigureConnectedAnchor = true;

            return component;
        }

        protected override Dictionary<string, string> _ToSrz()
        {
            throw new System.NotImplementedException();
        }

        protected override void _FromSrz(Dictionary<string, string> properties)
        {
            throw new System.NotImplementedException();
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
    }
}
