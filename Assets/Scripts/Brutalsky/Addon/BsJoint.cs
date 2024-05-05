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
        public override string Tag => Tags.JointLTag;
        public override bool HasLogic => true;

        public JointType JointType { get; private set; }
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
            AnchoredJoint2D component = JointType switch
            {
                JointType.Fixed => gameObject.AddComponent<FixedJoint2D>(),
                JointType.Distance => gameObject.AddComponent<DistanceJoint2D>(),
                JointType.Spring => gameObject.AddComponent<SpringJoint2D>(),
                JointType.Hinge => gameObject.AddComponent<HingeJoint2D>(),
                JointType.Slider => gameObject.AddComponent<SliderJoint2D>(),
                JointType.Wheel => gameObject.AddComponent<WheelJoint2D>(),
                _ => throw Errors.InvalidItem("joint type", JointType)
            };

            // Apply universal joint config
            component.anchor = Transform.Position;
            component.enableCollision = SelfCollision;
            component.breakForce = BreakForce;
            component.breakTorque = BreakTorque;

            // Apply specific joint config
            switch (JointType)
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
                    throw Errors.InvalidItem("joint type", JointType);
            }

            // Set up connected rigidbody
            component.autoConfigureConnectedAnchor = false;
            if (MountShape.Length > 0)
            {
                var mountShape = map.GetObject<BsShape>(Tags.ShapeLTag, MountShape);
                if (mountShape.InstanceObject == null)
                {
                    throw Errors.JointMountShapeUnbuilt(this);
                }
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
            return JointType switch
            {
                JointType.Fixed => RegisterLogicFixed(),
                JointType.Distance => RegisterLogicDistance(),
                JointType.Spring => RegisterLogicSpring(),
                JointType.Hinge => RegisterLogicHinge(),
                JointType.Slider => RegisterLogicSlider(),
                JointType.Wheel => RegisterLogicWheel(),
                _ => throw Errors.InvalidItem("joint type", JointType)
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

        protected override string[] _ToLcs()
        {
            var result = new List<string>
            {
                Stringifier.Str(LcsType.Transform, Transform),
                Stringifier.Str(LcsType.JointType, JointType),
                Stringifier.Str(LcsType.String, MountShape),
                Stringifier.Str(LcsType.Bool, SelfCollision),
                Stringifier.Str(LcsType.Float, BreakForce),
                Stringifier.Str(LcsType.Float, BreakTorque)
            };
            switch (JointType)
            {
                case JointType.Fixed:
                    result.Add(Stringifier.Str(LcsType.Float, DampingRatio));
                    result.Add(Stringifier.Str(LcsType.Float, DampingFrequency));
                    break;
                case JointType.Distance:
                    result.Add(Stringifier.Str(LcsType.Float, DistanceValue));
                    result.Add(Stringifier.Str(LcsType.Bool, DistanceAuto));
                    result.Add(Stringifier.Str(LcsType.Bool, DistanceMax));
                    break;
                case JointType.Spring:
                    result.Add(Stringifier.Str(LcsType.Float, DistanceValue));
                    result.Add(Stringifier.Str(LcsType.Bool, DistanceAuto));
                    result.Add(Stringifier.Str(LcsType.Float, DampingRatio));
                    result.Add(Stringifier.Str(LcsType.Float, DampingFrequency));
                    break;
                case JointType.Hinge:
                    result.Add(Stringifier.Str(LcsType.Bool, MotorEnabled));
                    result.Add(Stringifier.Str(LcsType.Float, MotorSpeed));
                    result.Add(Stringifier.Str(LcsType.Float, MotorForce));
                    result.Add(Stringifier.Str(LcsType.Bool, LimitEnabled));
                    result.Add(Stringifier.Str(LcsType.Float, LimitMin));
                    result.Add(Stringifier.Str(LcsType.Float, LimitMax));
                    break;
                case JointType.Slider:
                    result.Add(Stringifier.Str(LcsType.Float, AngleValue));
                    result.Add(Stringifier.Str(LcsType.Bool, AngleAuto));
                    result.Add(Stringifier.Str(LcsType.Bool, MotorEnabled));
                    result.Add(Stringifier.Str(LcsType.Float, MotorSpeed));
                    result.Add(Stringifier.Str(LcsType.Float, MotorForce));
                    result.Add(Stringifier.Str(LcsType.Bool, LimitEnabled));
                    result.Add(Stringifier.Str(LcsType.Float, LimitMin));
                    result.Add(Stringifier.Str(LcsType.Float, LimitMax));
                    break;
                case JointType.Wheel:
                    result.Add(Stringifier.Str(LcsType.Float, DampingRatio));
                    result.Add(Stringifier.Str(LcsType.Float, DampingFrequency));
                    result.Add(Stringifier.Str(LcsType.Float, AngleValue));
                    result.Add(Stringifier.Str(LcsType.Bool, AngleAuto));
                    result.Add(Stringifier.Str(LcsType.Bool, MotorEnabled));
                    result.Add(Stringifier.Str(LcsType.Float, MotorSpeed));
                    result.Add(Stringifier.Str(LcsType.Float, MotorForce));
                    break;
                default:
                    throw Errors.InvalidItem("joint type", JointType);
            }
            return result.ToArray();
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = Stringifier.Par<ObjectTransform>(LcsType.Transform, properties[0]);
            JointType = Stringifier.Par<JointType>(LcsType.JointType, properties[1]);
            MountShape = Stringifier.Par<string>(LcsType.String, properties[2]);
            SelfCollision = Stringifier.Par<bool>(LcsType.Bool, properties[3]);
            BreakForce = Stringifier.Par<float>(LcsType.Float, properties[4]);
            BreakTorque = Stringifier.Par<float>(LcsType.Float, properties[5]);
            switch (JointType)
            {
                case JointType.Fixed:
                    DampingRatio = Stringifier.Par<float>(LcsType.Float, properties[6]);
                    DampingFrequency = Stringifier.Par<float>(LcsType.Float, properties[7]);
                    break;
                case JointType.Distance:
                    DistanceValue = Stringifier.Par<float>(LcsType.Float, properties[6]);
                    DistanceAuto = Stringifier.Par<bool>(LcsType.Bool, properties[7]);
                    DistanceMax = Stringifier.Par<bool>(LcsType.Bool, properties[8]);
                    break;
                case JointType.Spring:
                    DistanceValue = Stringifier.Par<float>(LcsType.Float, properties[6]);
                    DistanceAuto = Stringifier.Par<bool>(LcsType.Bool, properties[7]);
                    DampingRatio = Stringifier.Par<float>(LcsType.Float, properties[8]);
                    DampingFrequency = Stringifier.Par<float>(LcsType.Float, properties[9]);
                    break;
                case JointType.Hinge:
                    MotorEnabled = Stringifier.Par<bool>(LcsType.Bool, properties[6]);
                    MotorSpeed = Stringifier.Par<float>(LcsType.Float, properties[7]);
                    MotorForce = Stringifier.Par<float>(LcsType.Float, properties[8]);
                    LimitEnabled = Stringifier.Par<bool>(LcsType.Bool, properties[9]);
                    LimitMin = Stringifier.Par<float>(LcsType.Float, properties[10]);
                    LimitMax = Stringifier.Par<float>(LcsType.Float, properties[11]);
                    break;
                case JointType.Slider:
                    AngleValue = Stringifier.Par<float>(LcsType.Float, properties[6]);
                    AngleAuto = Stringifier.Par<bool>(LcsType.Bool, properties[7]);
                    MotorEnabled = Stringifier.Par<bool>(LcsType.Bool, properties[8]);
                    MotorSpeed = Stringifier.Par<float>(LcsType.Float, properties[9]);
                    MotorForce = Stringifier.Par<float>(LcsType.Float, properties[10]);
                    LimitEnabled = Stringifier.Par<bool>(LcsType.Bool, properties[11]);
                    LimitMin = Stringifier.Par<float>(LcsType.Float, properties[12]);
                    LimitMax = Stringifier.Par<float>(LcsType.Float, properties[13]);
                    break;
                case JointType.Wheel:
                    DampingRatio = Stringifier.Par<float>(LcsType.Float, properties[6]);
                    DampingFrequency = Stringifier.Par<float>(LcsType.Float, properties[7]);
                    AngleValue = Stringifier.Par<float>(LcsType.Float, properties[8]);
                    AngleAuto = Stringifier.Par<bool>(LcsType.Bool, properties[9]);
                    MotorEnabled = Stringifier.Par<bool>(LcsType.Bool, properties[10]);
                    MotorSpeed = Stringifier.Par<float>(LcsType.Float, properties[11]);
                    MotorForce = Stringifier.Par<float>(LcsType.Float, properties[12]);
                    break;
                default:
                    throw Errors.InvalidItem("joint type", JointType);
            }
        }

        public BsJoint FixedJoint(float dampingRatio, float dampingFrequency)
        {
            JointType = JointType.Fixed;
            DampingRatio = dampingRatio;
            DampingFrequency = dampingFrequency;
            return this;
        }

        public BsJoint DistanceJoint(float distanceValue, bool distanceAuto, bool distanceMax)
        {
            JointType = JointType.Distance;
            DistanceValue = distanceValue;
            DistanceAuto = distanceAuto;
            DistanceMax = distanceMax;
            return this;
        }

        public BsJoint SpringJoint(float distanceValue, bool distanceAuto, float dampingRatio, float dampingFrequency)
        {
            JointType = JointType.Spring;
            DistanceValue = distanceValue;
            DistanceAuto = distanceAuto;
            DampingRatio = dampingRatio;
            DampingFrequency = dampingFrequency;
            return this;
        }

        public BsJoint HingeJoint(bool motorEnabled, float motorSpeed, float motorForce, bool limitEnabled,
            float limitMin, float limitMax)
        {
            JointType = JointType.Hinge;
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
            JointType = JointType.Slider;
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
            JointType = JointType.Wheel;
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
