using System;
using Data.Base;
using Data.Logic;
using Data.Object;
using UnityEngine;
using Utils.Constants;

namespace Data.Addon
{
    public class BsJoint : BsAddon
    {
        public const byte TypeFixed = 0;
        public const byte TypeDistance = 1;
        public const byte TypeSpring = 2;
        public const byte TypeHinge = 3;
        public const byte TypeSlider = 4;
        public const byte TypeWheel = 5;

        public override string Tag => Tags.JointPrefix;

        public byte Type { get; set; }
        public Vector2 Anchor { get; set; } = Vector2.zero;

        public string MountShape { get; set; } = "";
        public Vector2 MountAnchor { get; set; } = Vector2.zero;
        public bool MountCollision { get; set; } // Universal

        public float BreakForce { get; set; } = float.PositiveInfinity; // Universal
        public float BreakTorque { get; set; } = float.PositiveInfinity; // Universal

        public float AngleValue { get; set; } // Slider, Wheel
        public bool AngleAuto { get; set; } // Slider, Wheel

        public float DistanceValue { get; set; } // Distance, Spring
        public bool DistanceAuto { get; set; } // Distance, Spring
        public bool DistanceMax { get; set; } // Distance

        public float DampingRatio { get; set; } = .2f; // Fixed, Spring, Wheel
        public float DampingFrequency { get; set; } = 2f; // Fixed, Spring, Wheel

        public bool MotorEnabled { get; set; } // Hinge, Slider, Wheel
        public float MotorSpeed { get; set; } // Hinge, Slider, Wheel
        public float MotorForce { get; set; } // Hinge, Slider, Wheel

        public bool LimitEnabled { get; set; } // Hinge, Slider
        public float LimitMin { get; set; } // Hinge, Slider
        public float LimitMax { get; set; } // Hinge, Slider

        public BsJoint(string id = "") : base(id) { }

        protected override Component _Init(GameObject gameObject, BsObject parentObject, BsMap map)
        {
            // Create joint component
            AnchoredJoint2D component = Type switch
            {
                TypeFixed => gameObject.AddComponent<FixedJoint2D>(),
                TypeDistance => gameObject.AddComponent<DistanceJoint2D>(),
                TypeSpring => gameObject.AddComponent<SpringJoint2D>(),
                TypeHinge => gameObject.AddComponent<HingeJoint2D>(),
                TypeSlider => gameObject.AddComponent<SliderJoint2D>(),
                TypeWheel => gameObject.AddComponent<WheelJoint2D>(),
                _ => throw Errors.InvalidItem("joint type", Type)
            };

            // Apply universal joint config
            component.anchor = Anchor;
            component.breakForce = BreakForce;
            component.breakTorque = BreakTorque;

            // Apply specific joint config
            switch (Type)
            {
                case TypeFixed:
                    var fixedJointComponent = (FixedJoint2D)component;
                    fixedJointComponent.dampingRatio = DampingRatio;
                    fixedJointComponent.frequency = DampingFrequency;
                    break;

                case TypeDistance:
                    var distanceJointComponent = (DistanceJoint2D)component;
                    distanceJointComponent.distance = DistanceValue;
                    distanceJointComponent.autoConfigureDistance = DistanceAuto;
                    distanceJointComponent.maxDistanceOnly = DistanceMax;
                    break;

                case TypeSpring:
                    var springJointComponent = (SpringJoint2D)component;
                    springJointComponent.distance = DistanceValue;
                    springJointComponent.autoConfigureDistance = DistanceAuto;
                    springJointComponent.dampingRatio = DampingRatio;
                    springJointComponent.frequency = DampingFrequency;
                    break;

                case TypeHinge:
                    var hingeJointComponent = (HingeJoint2D)component;
                    if (MotorEnabled)
                    {
                        hingeJointComponent.useMotor = true;
                        var hingeJointMotor = hingeJointComponent.motor;
                        hingeJointMotor.motorSpeed = MotorSpeed;
                        hingeJointMotor.maxMotorTorque = MotorForce;
                        hingeJointComponent.motor = hingeJointMotor;
                    }
                    if (LimitEnabled)
                    {
                        hingeJointComponent.useLimits = true;
                        var hingeJointLimits = hingeJointComponent.limits;
                        hingeJointLimits.min = LimitMin;
                        hingeJointLimits.max = LimitMax;
                        hingeJointComponent.limits = hingeJointLimits;
                    }
                    break;

                case TypeSlider:
                    var sliderJointComponent = (SliderJoint2D)component;
                    sliderJointComponent.angle = AngleValue;
                    sliderJointComponent.autoConfigureAngle = AngleAuto;
                    if (MotorEnabled)
                    {
                        sliderJointComponent.useMotor = true;
                        var sliderJointMotor = sliderJointComponent.motor;
                        sliderJointMotor.motorSpeed = MotorSpeed;
                        sliderJointMotor.maxMotorTorque = MotorForce;
                        sliderJointComponent.motor = sliderJointMotor;
                    }
                    if (LimitEnabled)
                    {
                        sliderJointComponent.useLimits = true;
                        var sliderJointLimits = sliderJointComponent.limits;
                        sliderJointLimits.min = LimitMin;
                        sliderJointLimits.max = LimitMax;
                        sliderJointComponent.limits = sliderJointLimits;
                    }
                    break;

                case TypeWheel:
                    var wheelJointComponent = (WheelJoint2D)component;
                    var wheelJointSuspension = wheelJointComponent.suspension;
                    wheelJointSuspension.dampingRatio = DampingRatio;
                    wheelJointSuspension.frequency = DampingFrequency;
                    wheelJointSuspension.angle = AngleValue;
                    wheelJointComponent.suspension = wheelJointSuspension;
                    if (MotorEnabled)
                    {
                        wheelJointComponent.useMotor = true;
                        var wheelJointMotor = wheelJointComponent.motor;
                        wheelJointMotor.motorSpeed = MotorSpeed;
                        wheelJointMotor.maxMotorTorque = MotorForce;
                        wheelJointComponent.motor = wheelJointMotor;
                    }
                    break;

                default:
                    throw Errors.InvalidItem("joint type", Type);
            }

            // Set up connected rigidbody
            component.autoConfigureConnectedAnchor = false;
            if (MountShape.Length > 0)
            {
                component.enableCollision = MountCollision;
                var mountShape = map.GetObject<BsShape>(MountShape);
                if (!mountShape.InstanceObject) throw Errors.ParentObjectUnbuilt();
                component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
                component.connectedAnchor = MountAnchor;
            }
            component.connectedAnchor = MountAnchor;

            return component;
        }

        public override BsNode RegisterLogic()
        {
            return Type switch
            {
                TypeFixed => RegisterLogicFixed(),
                TypeDistance => RegisterLogicDistance(),
                TypeSpring => RegisterLogicSpring(),
                TypeHinge => RegisterLogicHinge(),
                TypeSlider => RegisterLogicSlider(),
                TypeWheel => RegisterLogicWheel(),
                _ => throw Errors.InvalidItem("joint type", Type)
            };
        }

        private BsNode RegisterLogicFixed()
        {
            var fixedJoint = (FixedJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[] { BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    fixedJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    fixedJoint.breakForce = inputs[1];
                    fixedJoint.breakTorque = inputs[2];
                    fixedJoint.dampingRatio = inputs[3];
                    fixedJoint.frequency = inputs[4];
                    return Array.Empty<float>();
                }
            };
        }

        private BsNode RegisterLogicDistance()
        {
            var distanceJoint = (DistanceJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[]
                    {
                        BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, DistanceValue,
                        BsNode.ToLogic(DistanceAuto), BsNode.ToLogic(DistanceMax)
                    },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    distanceJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    distanceJoint.breakForce = inputs[1];
                    distanceJoint.breakTorque = inputs[2];
                    distanceJoint.distance = inputs[3];
                    distanceJoint.autoConfigureDistance = BsNode.ToBool(inputs[4]);
                    distanceJoint.maxDistanceOnly = BsNode.ToBool(inputs[5]);
                    return Array.Empty<float>();
                }
            };
        }

        private BsNode RegisterLogicSpring()
        {
            var springJoint = (SpringJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[]
                    {
                        BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, DistanceValue,
                        BsNode.ToLogic(DistanceAuto), DampingRatio, DampingFrequency
                    },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    springJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    springJoint.breakForce = inputs[1];
                    springJoint.breakTorque = inputs[2];
                    springJoint.distance = inputs[3];
                    springJoint.autoConfigureDistance = BsNode.ToBool(inputs[4]);
                    springJoint.dampingRatio = inputs[5];
                    springJoint.frequency = inputs[6];
                    return Array.Empty<float>();
                }
            };
        }

        private BsNode RegisterLogicHinge()
        {
            var hingeJoint = (HingeJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[]
                    {
                        BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, BsNode.ToLogic(MotorEnabled),
                        MotorSpeed, MotorForce, BsNode.ToLogic(LimitEnabled), LimitMin, LimitMax
                    },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    hingeJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    hingeJoint.breakForce = inputs[1];
                    hingeJoint.breakTorque = inputs[2];
                    hingeJoint.useMotor = BsNode.ToBool(inputs[3]);
                    if (hingeJoint.useMotor)
                    {
                        var motor = hingeJoint.motor;
                        motor.motorSpeed = inputs[4];
                        motor.maxMotorTorque = inputs[5];
                        hingeJoint.motor = motor;
                    }
                    hingeJoint.useLimits = BsNode.ToBool(inputs[6]);
                    if (hingeJoint.useLimits)
                    {
                        var limits = hingeJoint.limits;
                        limits.min = inputs[7];
                        limits.max = inputs[8];
                        hingeJoint.limits = limits;
                    }
                    return Array.Empty<float>();
                }
            };
        }

        private BsNode RegisterLogicSlider()
        {
            var sliderJoint = (SliderJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[]
                    {
                        BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, AngleValue, BsNode.ToLogic(AngleAuto),
                        BsNode.ToLogic(MotorEnabled), MotorSpeed, MotorForce, BsNode.ToLogic(LimitEnabled), LimitMin,
                        LimitMax
                    },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    sliderJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    sliderJoint.breakForce = inputs[1];
                    sliderJoint.breakTorque = inputs[2];
                    sliderJoint.angle = inputs[3];
                    sliderJoint.autoConfigureAngle = BsNode.ToBool(inputs[4]);
                    sliderJoint.useMotor = BsNode.ToBool(inputs[5]);
                    if (sliderJoint.useMotor)
                    {
                        var motor = sliderJoint.motor;
                        motor.motorSpeed = inputs[6];
                        motor.maxMotorTorque = inputs[7];
                        sliderJoint.motor = motor;
                    }
                    sliderJoint.useLimits = BsNode.ToBool(inputs[8]);
                    if (sliderJoint.useLimits)
                    {
                        var limits = sliderJoint.limits;
                        limits.min = inputs[9];
                        limits.max = inputs[10];
                        sliderJoint.limits = limits;
                    }
                    return Array.Empty<float>();
                }
            };
        }

        private BsNode RegisterLogicWheel()
        {
            var wheelJoint = (WheelJoint2D)InstanceComponent;
            return new BsNode(Tags.JointPrefix, Id)
            {
                Init = () =>
                (
                    new[]
                    {
                        BsNode.ToLogic(MountCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency,
                        AngleValue, BsNode.ToLogic(MotorEnabled), MotorSpeed, MotorForce
                    },
                    Array.Empty<float>()
                ),
                Update = inputs =>
                {
                    wheelJoint.enableCollision = BsNode.ToBool(inputs[0]);
                    wheelJoint.breakForce = inputs[1];
                    wheelJoint.breakTorque = inputs[2];
                    var suspension = wheelJoint.suspension;
                    suspension.dampingRatio = inputs[3];
                    suspension.frequency = inputs[4];
                    suspension.angle = inputs[5];
                    wheelJoint.suspension = suspension;
                    wheelJoint.useMotor = BsNode.ToBool(inputs[6]);
                    if (wheelJoint.useMotor)
                    {
                        var motor = wheelJoint.motor;
                        motor.motorSpeed = inputs[7];
                        motor.maxMotorTorque = inputs[8];
                        wheelJoint.motor = motor;
                    }
                    return Array.Empty<float>();
                }
            };
        }
    }
}
