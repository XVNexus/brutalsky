using System.Collections.Generic;
using Brutalsky.Base;
using Brutalsky.Logic;
using Brutalsky.Object;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Lcs;
using Utils.Object;

namespace Brutalsky.Addon
{
    public class BsJoint : BsAddon
    {
        public override string Tag => Tags.JointPrefix;
        public override bool HasLogic => true;

        public JointType Type { get; private set; }
        public string MountShape { get; set; }

        public bool SelfCollision { get; set; } // Universal

        public float BreakForce { get; set; } // Universal
        public float BreakTorque { get; set; } // Universal

        public float AngleValue { get; set; } // Slider, Wheel
        public bool AngleAuto { get; set; } // Slider, Wheel

        public float DistanceValue { get; set; } // Distance, Spring
        public bool DistanceAuto { get; set; } // Distance, Spring
        public bool DistanceMax { get; set; } // Distance

        public float DampingRatio { get; set; } // Fixed, Spring, Wheel
        public float DampingFrequency { get; set; } // Fixed, Spring, Wheel

        public bool MotorEnabled { get; set; } // Hinge, Slider, Wheel
        public float MotorSpeed { get; set; } // Hinge, Slider, Wheel
        public float MotorForce { get; set; } // Hinge, Slider, Wheel

        public bool LimitEnabled { get; set; } // Hinge, Slider
        public float LimitMin { get; set; } // Hinge, Slider
        public float LimitMax { get; set; } // Hinge, Slider

        public BsJoint(string id, ObjectTransform transform, string mountShape, bool selfCollision,
            float breakForce, float breakTorque) : base(id, transform)
        {
            MountShape = mountShape;
            SelfCollision = selfCollision;
            BreakForce = breakForce;
            BreakTorque = breakTorque;
        }

        public BsJoint()
        {
        }

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
            component.anchor = Transform.Position;
            component.enableCollision = SelfCollision;
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
                var mountShape = map.GetObject<BsShape>(Tags.ShapePrefix, MountShape);
                if (!mountShape.InstanceObject) throw Errors.ParentObjectUnbuilt();
                component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
                component.connectedAnchor = parentObject.Transform.Position - mountShape.Transform.Position;
            }
            else
            {
                component.connectedAnchor = parentObject.Transform.Position;
            }

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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency
                },
                new float[5], (inputs, _) =>
                {
                    fixedJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    fixedJoint.breakForce = inputs[1];
                    fixedJoint.breakTorque = inputs[2];
                    fixedJoint.dampingRatio = inputs[3];
                    fixedJoint.frequency = inputs[4];
                    return new[]
                    {
                        BsMatrix.ToLogic(fixedJoint.enableCollision), fixedJoint.breakForce, fixedJoint.breakTorque,
                        fixedJoint.dampingRatio, fixedJoint.frequency
                    };
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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, DistanceValue,
                    BsMatrix.ToLogic(DistanceAuto), BsMatrix.ToLogic(DistanceMax)
                },
                new float[6],
                (inputs, _) =>
                {
                    distanceJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    distanceJoint.breakForce = inputs[1];
                    distanceJoint.breakTorque = inputs[2];
                    distanceJoint.distance = inputs[3];
                    distanceJoint.autoConfigureDistance = BsMatrix.ToBool(inputs[4]);
                    distanceJoint.maxDistanceOnly = BsMatrix.ToBool(inputs[5]);
                    return new[]
                    {
                        BsMatrix.ToLogic(distanceJoint.enableCollision), distanceJoint.breakForce,
                        distanceJoint.breakTorque, distanceJoint.distance,
                        BsMatrix.ToLogic(distanceJoint.autoConfigureDistance),
                        BsMatrix.ToLogic(distanceJoint.maxDistanceOnly)
                    };
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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, DistanceValue,
                    BsMatrix.ToLogic(DistanceAuto), DampingRatio, DampingFrequency
                },
                new float[7],
                (inputs, _) =>
                {
                    springJoint.enableCollision = BsMatrix.ToBool(inputs[0]);
                    springJoint.breakForce = inputs[1];
                    springJoint.breakTorque = inputs[2];
                    springJoint.distance = inputs[3];
                    springJoint.autoConfigureDistance = BsMatrix.ToBool(inputs[4]);
                    springJoint.dampingRatio = inputs[5];
                    springJoint.frequency = inputs[6];
                    return new[]
                    {
                        BsMatrix.ToLogic(springJoint.enableCollision), springJoint.breakForce,
                        springJoint.breakTorque, springJoint.distance,
                        BsMatrix.ToLogic(springJoint.autoConfigureDistance), springJoint.dampingRatio,
                        springJoint.frequency
                    };
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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, BsMatrix.ToLogic(MotorEnabled),
                    MotorSpeed, MotorForce, BsMatrix.ToLogic(LimitEnabled), LimitMin, LimitMax
                },
                new float[9],
                (inputs, _) =>
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
                    return new[]
                    {
                        BsMatrix.ToLogic(hingeJoint.enableCollision), hingeJoint.breakForce, hingeJoint.breakTorque,
                        BsMatrix.ToLogic(hingeJoint.useMotor), hingeJoint.motor.motorSpeed,
                        hingeJoint.motor.maxMotorTorque, BsMatrix.ToLogic(hingeJoint.useLimits),
                        hingeJoint.limits.min, hingeJoint.limits.max
                    };
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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, AngleValue,
                    BsMatrix.ToLogic(AngleAuto), BsMatrix.ToLogic(MotorEnabled), MotorSpeed, MotorForce,
                    BsMatrix.ToLogic(LimitEnabled), LimitMin, LimitMax
                },
                new float[11],
                (inputs, _) =>
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
                    return new[]
                    {
                        BsMatrix.ToLogic(sliderJoint.enableCollision), sliderJoint.breakForce,
                        sliderJoint.breakTorque, sliderJoint.angle, BsMatrix.ToLogic(sliderJoint.autoConfigureAngle),
                        BsMatrix.ToLogic(sliderJoint.useMotor), sliderJoint.motor.motorSpeed,
                        sliderJoint.motor.maxMotorTorque, BsMatrix.ToLogic(sliderJoint.useLimits),
                        sliderJoint.limits.min, sliderJoint.limits.max
                    };
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
                    BsMatrix.ToLogic(SelfCollision), BreakForce, BreakTorque, DampingRatio, DampingFrequency,
                    AngleValue, BsMatrix.ToLogic(MotorEnabled), MotorSpeed, MotorForce
                },
                new float[9],
                (inputs, _) =>
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
                    return new[]
                    {
                        BsMatrix.ToLogic(wheelJoint.enableCollision), wheelJoint.breakForce, wheelJoint.breakTorque,
                        wheelJoint.suspension.dampingRatio, wheelJoint.suspension.frequency,
                        wheelJoint.suspension.angle, BsMatrix.ToLogic(wheelJoint.useMotor),
                        wheelJoint.motor.motorSpeed, wheelJoint.motor.maxMotorTorque
                    };
                }
            );
        }

        protected override LcsProp[] _ToLcs()
        {
            var result = new List<LcsProp>
            {
                new(LcsType.JointType, Type),
                new(LcsType.String, MountShape),
                new(LcsType.Bool, SelfCollision),
                new(LcsType.Float, BreakForce),
                new(LcsType.Float, BreakTorque)
            };
            switch (Type)
            {
                case JointType.Fixed:
                    result.Add(new LcsProp(LcsType.Float, DampingRatio));
                    result.Add(new LcsProp(LcsType.Float, DampingFrequency));
                    break;
                case JointType.Distance:
                    result.Add(new LcsProp(LcsType.Float, DistanceValue));
                    result.Add(new LcsProp(LcsType.Bool, DistanceAuto));
                    result.Add(new LcsProp(LcsType.Bool, DistanceMax));
                    break;
                case JointType.Spring:
                    result.Add(new LcsProp(LcsType.Float, DistanceValue));
                    result.Add(new LcsProp(LcsType.Bool, DistanceAuto));
                    result.Add(new LcsProp(LcsType.Float, DampingRatio));
                    result.Add(new LcsProp(LcsType.Float, DampingFrequency));
                    break;
                case JointType.Hinge:
                    result.Add(new LcsProp(LcsType.Bool, MotorEnabled));
                    result.Add(new LcsProp(LcsType.Float, MotorSpeed));
                    result.Add(new LcsProp(LcsType.Float, MotorForce));
                    result.Add(new LcsProp(LcsType.Bool, LimitEnabled));
                    result.Add(new LcsProp(LcsType.Float, LimitMin));
                    result.Add(new LcsProp(LcsType.Float, LimitMax));
                    break;
                case JointType.Slider:
                    result.Add(new LcsProp(LcsType.Float, AngleValue));
                    result.Add(new LcsProp(LcsType.Bool, AngleAuto));
                    result.Add(new LcsProp(LcsType.Bool, MotorEnabled));
                    result.Add(new LcsProp(LcsType.Float, MotorSpeed));
                    result.Add(new LcsProp(LcsType.Float, MotorForce));
                    result.Add(new LcsProp(LcsType.Bool, LimitEnabled));
                    result.Add(new LcsProp(LcsType.Float, LimitMin));
                    result.Add(new LcsProp(LcsType.Float, LimitMax));
                    break;
                case JointType.Wheel:
                    result.Add(new LcsProp(LcsType.Float, DampingRatio));
                    result.Add(new LcsProp(LcsType.Float, DampingFrequency));
                    result.Add(new LcsProp(LcsType.Float, AngleValue));
                    result.Add(new LcsProp(LcsType.Bool, AngleAuto));
                    result.Add(new LcsProp(LcsType.Bool, MotorEnabled));
                    result.Add(new LcsProp(LcsType.Float, MotorSpeed));
                    result.Add(new LcsProp(LcsType.Float, MotorForce));
                    break;
                default:
                    throw Errors.InvalidItem("joint type", Type);
            }
            return result.ToArray();
        }

        protected override void _FromLcs(LcsProp[] props)
        {
            Type = (JointType)props[0].Value;
            MountShape = (string)props[1].Value;
            SelfCollision = (bool)props[2].Value;
            BreakForce = (float)props[3].Value;
            BreakTorque = (float)props[4].Value;
            switch (Type)
            {
                case JointType.Fixed:
                    DampingRatio = (float)props[5].Value;
                    DampingFrequency = (float)props[6].Value;
                    break;
                case JointType.Distance:
                    DistanceValue = (float)props[5].Value;
                    DistanceAuto = (bool)props[6].Value;
                    DistanceMax = (bool)props[7].Value;
                    break;
                case JointType.Spring:
                    DistanceValue = (float)props[5].Value;
                    DistanceAuto = (bool)props[6].Value;
                    DampingRatio = (float)props[7].Value;
                    DampingFrequency = (float)props[8].Value;
                    break;
                case JointType.Hinge:
                    MotorEnabled = (bool)props[5].Value;
                    MotorSpeed = (float)props[6].Value;
                    MotorForce = (float)props[7].Value;
                    LimitEnabled = (bool)props[8].Value;
                    LimitMin = (float)props[9].Value;
                    LimitMax = (float)props[10].Value;
                    break;
                case JointType.Slider:
                    AngleValue = (float)props[5].Value;
                    AngleAuto = (bool)props[6].Value;
                    MotorEnabled = (bool)props[7].Value;
                    MotorSpeed = (float)props[8].Value;
                    MotorForce = (float)props[9].Value;
                    LimitEnabled = (bool)props[10].Value;
                    LimitMin = (float)props[11].Value;
                    LimitMax = (float)props[12].Value;
                    break;
                case JointType.Wheel:
                    DampingRatio = (float)props[5].Value;
                    DampingFrequency = (float)props[6].Value;
                    AngleValue = (float)props[7].Value;
                    AngleAuto = (bool)props[8].Value;
                    MotorEnabled = (bool)props[9].Value;
                    MotorSpeed = (float)props[10].Value;
                    MotorForce = (float)props[11].Value;
                    break;
                default:
                    throw Errors.InvalidItem("joint type", Type);
            }
        }

        public BsJoint FixedJoint(float dampingRatio, float dampingFrequency)
        {
            Type = JointType.Fixed;
            DampingRatio = dampingRatio;
            DampingFrequency = dampingFrequency;
            return this;
        }

        public BsJoint DistanceJoint(float distanceValue, bool distanceAuto, bool distanceMax)
        {
            Type = JointType.Distance;
            DistanceValue = distanceValue;
            DistanceAuto = distanceAuto;
            DistanceMax = distanceMax;
            return this;
        }

        public BsJoint SpringJoint(float distanceValue, bool distanceAuto, float dampingRatio, float dampingFrequency)
        {
            Type = JointType.Spring;
            DistanceValue = distanceValue;
            DistanceAuto = distanceAuto;
            DampingRatio = dampingRatio;
            DampingFrequency = dampingFrequency;
            return this;
        }

        public BsJoint HingeJoint(bool motorEnabled, float motorSpeed, float motorForce, bool limitEnabled,
            float limitMin, float limitMax)
        {
            Type = JointType.Hinge;
            MotorEnabled = motorEnabled;
            MotorSpeed = motorSpeed;
            MotorForce = motorForce;
            LimitEnabled = limitEnabled;
            LimitMin = limitMin;
            LimitMax = limitMax;
            return this;
        }

        public BsJoint SliderJoint(float angleValue, bool angleAuto, bool motorEnabled, float motorSpeed,
            float motorForce, bool limitEnabled, float limitMin, float limitMax)
        {
            Type = JointType.Slider;
            AngleValue = angleValue;
            AngleAuto = angleAuto;
            MotorEnabled = motorEnabled;
            MotorSpeed = motorSpeed;
            MotorForce = motorForce;
            LimitEnabled = limitEnabled;
            LimitMin = limitMin;
            LimitMax = limitMax;
            return this;
        }

        public BsJoint WheelJoint(float dampingRatio, float dampingFrequency, float angleValue, bool angleAuto,
            bool motorEnabled, float motorSpeed, float motorForce)
        {
            Type = JointType.Wheel;
            DampingRatio = dampingRatio;
            DampingFrequency = dampingFrequency;
            AngleValue = angleValue;
            AngleAuto = angleAuto;
            MotorEnabled = motorEnabled;
            MotorSpeed = motorSpeed;
            MotorForce = motorForce;
            return this;
        }
    }
}
