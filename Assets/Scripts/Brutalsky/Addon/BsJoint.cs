using System;
using System.Collections.Generic;
using Brutalsky.Base;
using Brutalsky.Logic;
using Brutalsky.Object;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Lcs;

namespace Brutalsky.Addon
{
    public class BsJoint : BsAddon
    {
        public override string Tag => Tags.JointPrefix;

        public JointType Type { get; set; }
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
                JointType.Fixed => gameObject.AddComponent<FixedJoint2D>(),
                JointType.Distance => gameObject.AddComponent<DistanceJoint2D>(),
                JointType.Spring => gameObject.AddComponent<SpringJoint2D>(),
                JointType.Hinge => gameObject.AddComponent<HingeJoint2D>(),
                JointType.Slider => gameObject.AddComponent<SliderJoint2D>(),
                JointType.Wheel => gameObject.AddComponent<WheelJoint2D>(),
                _ => throw Errors.InvalidItem("joint type", Type)
            };

            // Apply universal joint config
            component.anchor = Anchor;
            component.breakForce = BreakForce;
            component.breakTorque = BreakTorque;

            // Apply specific joint config
            switch (Type)
            {
                case JointType.Fixed:
                    var fixedJointComponent = (FixedJoint2D)component;
                    fixedJointComponent.dampingRatio = DampingRatio;
                    fixedJointComponent.frequency = DampingFrequency;
                    break;

                case JointType.Distance:
                    var distanceJointComponent = (DistanceJoint2D)component;
                    distanceJointComponent.distance = DistanceValue;
                    distanceJointComponent.autoConfigureDistance = DistanceAuto;
                    distanceJointComponent.maxDistanceOnly = DistanceMax;
                    break;

                case JointType.Spring:
                    var springJointComponent = (SpringJoint2D)component;
                    springJointComponent.distance = DistanceValue;
                    springJointComponent.autoConfigureDistance = DistanceAuto;
                    springJointComponent.dampingRatio = DampingRatio;
                    springJointComponent.frequency = DampingFrequency;
                    break;

                case JointType.Hinge:
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

                case JointType.Slider:
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

                case JointType.Wheel:
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
                var mountShape = map.GetObject<BsShape>(Tags.ShapePrefix, MountShape);
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
                JointType.Fixed => RegisterLogicFixed(),
                JointType.Distance => RegisterLogicDistance(),
                JointType.Spring => RegisterLogicSpring(),
                JointType.Hinge => RegisterLogicHinge(),
                JointType.Slider => RegisterLogicSlider(),
                JointType.Wheel => RegisterLogicWheel(),
                _ => throw Errors.InvalidItem("joint type", Type)
            };
        }

        private BsNode RegisterLogicFixed()
        {
            var fixedJoint = (FixedJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    fixedJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    fixedJoint.breakForce = inputs[1];
                    fixedJoint.breakTorque = inputs[2];
                    fixedJoint.dampingRatio = inputs[3];
                    fixedJoint.frequency = inputs[4];
                    return Array.Empty<float>();
                }
            );
        }

        private BsNode RegisterLogicDistance()
        {
            var distanceJoint = (DistanceJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, DistanceValue,
                    BsMatrix.ToLogic(DistanceAuto), BsMatrix.ToLogic(DistanceMax)
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    distanceJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    distanceJoint.breakForce = inputs[1];
                    distanceJoint.breakTorque = inputs[2];
                    distanceJoint.distance = inputs[3];
                    distanceJoint.autoConfigureDistance = BsMatrix.ToBool(inputs[4]);
                    distanceJoint.maxDistanceOnly = BsMatrix.ToBool(inputs[5]);
                    return Array.Empty<float>();
                }
            );
        }

        private BsNode RegisterLogicSpring()
        {
            var springJoint = (SpringJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, DistanceValue,
                    BsMatrix.ToLogic(DistanceAuto), DampingRatio, DampingFrequency
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    springJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    springJoint.breakForce = inputs[1];
                    springJoint.breakTorque = inputs[2];
                    springJoint.distance = inputs[3];
                    springJoint.autoConfigureDistance = BsMatrix.ToBool(inputs[4]);
                    springJoint.dampingRatio = inputs[5];
                    springJoint.frequency = inputs[6];
                    return Array.Empty<float>();
                }
            );
        }

        private BsNode RegisterLogicHinge()
        {
            var hingeJoint = (HingeJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, BsMatrix.ToLogic(MotorEnabled),
                    MotorSpeed, MotorForce, BsMatrix.ToLogic(LimitEnabled), LimitMin, LimitMax
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    hingeJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    hingeJoint.breakForce = inputs[1];
                    hingeJoint.breakTorque = inputs[2];
                    hingeJoint.useMotor = BsMatrix.ToBool(inputs[3]);
                    if (hingeJoint.useMotor)
                    {
                        var motor = hingeJoint.motor;
                        motor.motorSpeed = inputs[4];
                        motor.maxMotorTorque = inputs[5];
                        hingeJoint.motor = motor;
                    }
                    hingeJoint.useLimits = BsMatrix.ToBool(inputs[6]);
                    if (hingeJoint.useLimits)
                    {
                        var limits = hingeJoint.limits;
                        limits.min = inputs[7];
                        limits.max = inputs[8];
                        hingeJoint.limits = limits;
                    }
                    return Array.Empty<float>();
                }
            );
        }

        private BsNode RegisterLogicSlider()
        {
            var sliderJoint = (SliderJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, AngleValue,
                    BsMatrix.ToLogic(AngleAuto), BsMatrix.ToLogic(MotorEnabled), MotorSpeed, MotorForce,
                    BsMatrix.ToLogic(LimitEnabled), LimitMin, LimitMax
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    sliderJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    sliderJoint.breakForce = inputs[1];
                    sliderJoint.breakTorque = inputs[2];
                    sliderJoint.angle = inputs[3];
                    sliderJoint.autoConfigureAngle = BsMatrix.ToBool(inputs[4]);
                    sliderJoint.useMotor = BsMatrix.ToBool(inputs[5]);
                    if (sliderJoint.useMotor)
                    {
                        var motor = sliderJoint.motor;
                        motor.motorSpeed = inputs[6];
                        motor.maxMotorTorque = inputs[7];
                        sliderJoint.motor = motor;
                    }
                    sliderJoint.useLimits = BsMatrix.ToBool(inputs[8]);
                    if (sliderJoint.useLimits)
                    {
                        var limits = sliderJoint.limits;
                        limits.min = inputs[9];
                        limits.max = inputs[10];
                        sliderJoint.limits = limits;
                    }
                    return Array.Empty<float>();
                }
            );
        }

        private BsNode RegisterLogicWheel()
        {
            var wheelJoint = (WheelJoint2D)InstanceComponent;
            return new BsNode
            (
                new[]
                {
                    BsMatrix.ToLogic(MountCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency,
                    AngleValue, BsMatrix.ToLogic(MotorEnabled), MotorSpeed, MotorForce
                },
                Array.Empty<float>(), (inputs, _) =>
                {
                    wheelJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    wheelJoint.breakForce = inputs[1];
                    wheelJoint.breakTorque = inputs[2];
                    var suspension = wheelJoint.suspension;
                    suspension.dampingRatio = inputs[3];
                    suspension.frequency = inputs[4];
                    suspension.angle = inputs[5];
                    wheelJoint.suspension = suspension;
                    wheelJoint.useMotor = BsMatrix.ToBool(inputs[6]);
                    if (wheelJoint.useMotor)
                    {
                        var motor = wheelJoint.motor;
                        motor.motorSpeed = inputs[7];
                        motor.maxMotorTorque = inputs[8];
                        wheelJoint.motor = motor;
                    }
                    return Array.Empty<float>();
                }
            );
        }

        protected override LcsProp[] _ToLcs()
        {
            var result = new List<LcsProp>
            {
                new(Type),
                new(Anchor),
                new(MountShape),
                new(MountAnchor),
                new(MountCollision),
                new(BreakForce),
                new(BreakTorque)
            };
            switch (Type)
            {
                case JointType.Fixed:
                    result.Add(new LcsProp(DampingRatio));
                    result.Add(new LcsProp(DampingFrequency));
                    break;
                case JointType.Distance:
                    result.Add(new LcsProp(DistanceValue));
                    result.Add(new LcsProp(DistanceAuto));
                    result.Add(new LcsProp(DistanceMax));
                    break;
                case JointType.Spring:
                    result.Add(new LcsProp(DistanceValue));
                    result.Add(new LcsProp(DistanceAuto));
                    result.Add(new LcsProp(DampingRatio));
                    result.Add(new LcsProp(DampingFrequency));
                    break;
                case JointType.Hinge:
                    result.Add(new LcsProp(MotorEnabled));
                    result.Add(new LcsProp(MotorSpeed));
                    result.Add(new LcsProp(MotorForce));
                    result.Add(new LcsProp(LimitEnabled));
                    result.Add(new LcsProp(LimitMin));
                    result.Add(new LcsProp(LimitMax));
                    break;
                case JointType.Slider:
                    result.Add(new LcsProp(AngleValue));
                    result.Add(new LcsProp(AngleAuto));
                    result.Add(new LcsProp(MotorEnabled));
                    result.Add(new LcsProp(MotorSpeed));
                    result.Add(new LcsProp(MotorForce));
                    result.Add(new LcsProp(LimitEnabled));
                    result.Add(new LcsProp(LimitMin));
                    result.Add(new LcsProp(LimitMax));
                    break;
                case JointType.Wheel:
                    result.Add(new LcsProp(DampingRatio));
                    result.Add(new LcsProp(DampingFrequency));
                    result.Add(new LcsProp(AngleValue));
                    result.Add(new LcsProp(AngleAuto));
                    result.Add(new LcsProp(MotorEnabled));
                    result.Add(new LcsProp(MotorSpeed));
                    result.Add(new LcsProp(MotorForce));
                    break;
                default:
                    throw Errors.InvalidItem("joint type", Type);
            }
            return result.ToArray();
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            var i = 0;
            Type = (JointType)props[i++].Value;
            Anchor = (Vector2)props[i++].Value;
            MountShape = (string)props[i++].Value;
            MountAnchor = (Vector2)props[i++].Value;
            MountCollision = (bool)props[i++].Value;
            BreakForce = (float)props[i++].Value;
            BreakTorque = (float)props[i++].Value;
            switch (Type)
            {
                case JointType.Fixed:
                    DampingRatio = (float)props[i++].Value;
                    DampingFrequency = (float)props[i++].Value;
                    break;
                case JointType.Distance:
                    DistanceValue = (float)props[i++].Value;
                    DistanceAuto = (bool)props[i++].Value;
                    DistanceMax = (bool)props[i++].Value;
                    break;
                case JointType.Spring:
                    DistanceValue = (float)props[i++].Value;
                    DistanceAuto = (bool)props[i++].Value;
                    DampingRatio = (float)props[i++].Value;
                    DampingFrequency = (float)props[i++].Value;
                    break;
                case JointType.Hinge:
                    MotorEnabled = (bool)props[i++].Value;
                    MotorSpeed = (float)props[i++].Value;
                    MotorForce = (float)props[i++].Value;
                    LimitEnabled = (bool)props[i++].Value;
                    LimitMin = (float)props[i++].Value;
                    LimitMax = (float)props[i++].Value;
                    break;
                case JointType.Slider:
                    AngleValue = (float)props[i++].Value;
                    AngleAuto = (bool)props[i++].Value;
                    MotorEnabled = (bool)props[i++].Value;
                    MotorSpeed = (float)props[i++].Value;
                    MotorForce = (float)props[i++].Value;
                    LimitEnabled = (bool)props[i++].Value;
                    LimitMin = (float)props[i++].Value;
                    LimitMax = (float)props[i++].Value;
                    break;
                case JointType.Wheel:
                    DampingRatio = (float)props[i++].Value;
                    DampingFrequency = (float)props[i++].Value;
                    AngleValue = (float)props[i++].Value;
                    AngleAuto = (bool)props[i++].Value;
                    MotorEnabled = (bool)props[i++].Value;
                    MotorSpeed = (float)props[i++].Value;
                    MotorForce = (float)props[i++].Value;
                    break;
                default:
                    throw Errors.InvalidItem("joint type", Type);
            }
        }
    }
}
