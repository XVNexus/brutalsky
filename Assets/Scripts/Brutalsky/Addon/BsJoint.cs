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
        public override char Tag => Tags.JointSym;

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
                _ => throw Errors.InvalidJointType(JointType)
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
                    throw Errors.InvalidJointType(JointType);
            }

            // Set up connected rigidbody
            if (MountShape.Length > 0)
            {
                var mountShape = map.GetObject<BsShape>(Tags.ShapeSym, MountShape);
                if (mountShape.InstanceObject == null)
                {
                    throw Errors.JointMountShapeUnbuilt(this);
                }
                component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
                component.autoConfigureConnectedAnchor = true;
            }

            return component;
        }

        protected override void _RegisterLogic(BsMatrix matrix)
        {
            var joint = (AnchoredJoint2D)InstanceComponent;
            matrix.AddPort(new BsPort(Id, "enable-collision",
                () => BsMatrix.Bool2Logic(joint.enableCollision),
                value => joint.enableCollision = BsMatrix.Logic2Bool(value)
            ));
            matrix.AddPort(new BsPort(Id, "break-force",
                () => joint.breakForce,
                value => joint.breakForce = value
            ));
            matrix.AddPort(new BsPort(Id, "break-torque",
                () => joint.breakTorque,
                value => joint.breakTorque = value
            ));

            switch (JointType)
            {
                case JointType.Fixed:
                    var fixedJoint = (FixedJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "damping-ratio",
                        () => fixedJoint.dampingRatio,
                        value => fixedJoint.dampingRatio = value
                    ));
                    matrix.AddPort(new BsPort(Id, "damping-frequency",
                        () => fixedJoint.frequency,
                        value => fixedJoint.frequency = value
                    ));
                    break;

                case JointType.Distance:
                    var distanceJoint = (DistanceJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "distance-value",
                        () => distanceJoint.distance,
                        value => distanceJoint.distance = value
                    ));
                    matrix.AddPort(new BsPort(Id, "distance-auto",
                        () => BsMatrix.Bool2Logic(distanceJoint.autoConfigureDistance),
                        value => distanceJoint.autoConfigureDistance = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "distance-max",
                        () => BsMatrix.Bool2Logic(distanceJoint.maxDistanceOnly),
                        value => distanceJoint.maxDistanceOnly = BsMatrix.Logic2Bool(value)
                    ));
                    break;

                case JointType.Spring:
                    var springJoint = (SpringJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "distance-value",
                        () => springJoint.distance,
                        value => springJoint.distance = value
                    ));
                    matrix.AddPort(new BsPort(Id, "distance-auto",
                        () => BsMatrix.Bool2Logic(springJoint.autoConfigureDistance),
                        value => springJoint.autoConfigureDistance = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "damping-ratio",
                        () => springJoint.dampingRatio,
                        value => springJoint.dampingRatio = value
                    ));
                    matrix.AddPort(new BsPort(Id, "damping-frequency",
                        () => springJoint.frequency,
                        value => springJoint.frequency = value
                    ));
                    break;

                case JointType.Hinge:
                    var hingeJoint = (HingeJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "motor-enabled",
                        () => BsMatrix.Bool2Logic(hingeJoint.useMotor),
                        value => hingeJoint.useMotor = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-speed",
                        () => hingeJoint.motor.motorSpeed,
                        value =>
                        {
                            var motor = hingeJoint.motor;
                            motor.motorSpeed = value;
                            hingeJoint.motor = motor;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-force",
                        () => hingeJoint.motor.maxMotorTorque,
                        value =>
                        {
                            var motor = hingeJoint.motor;
                            motor.maxMotorTorque = value;
                            hingeJoint.motor = motor;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-enabled",
                        () => BsMatrix.Bool2Logic(hingeJoint.useLimits),
                        value => hingeJoint.useLimits = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-min",
                        () => hingeJoint.limits.min,
                        value =>
                        {
                            var limits = hingeJoint.limits;
                            limits.min = value;
                            hingeJoint.limits = limits;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-max",
                        () => hingeJoint.limits.max,
                        value =>
                        {
                            var limits = hingeJoint.limits;
                            limits.max = value;
                            hingeJoint.limits = limits;
                        }
                    ));
                    break;

                case JointType.Slider:
                    var sliderJoint = (SliderJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "angle-value",
                        () => sliderJoint.angle,
                        value => sliderJoint.angle = value
                    ));
                    matrix.AddPort(new BsPort(Id, "angle-auto",
                        () => BsMatrix.Bool2Logic(sliderJoint.autoConfigureAngle),
                        value => sliderJoint.autoConfigureAngle = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-enabled",
                        () => BsMatrix.Bool2Logic(sliderJoint.useMotor),
                        value => sliderJoint.useMotor = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-speed",
                        () => sliderJoint.motor.motorSpeed,
                        value =>
                        {
                            var motor = sliderJoint.motor;
                            motor.motorSpeed = value;
                            sliderJoint.motor = motor;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-force",
                        () => sliderJoint.motor.maxMotorTorque,
                        value =>
                        {
                            var motor = sliderJoint.motor;
                            motor.maxMotorTorque = value;
                            sliderJoint.motor = motor;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-enabled",
                        () => BsMatrix.Bool2Logic(sliderJoint.useLimits),
                        value => sliderJoint.useLimits = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-min",
                        () => sliderJoint.limits.min,
                        value =>
                        {
                            var limits = sliderJoint.limits;
                            limits.min = value;
                            sliderJoint.limits = limits;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "limit-max",
                        () => sliderJoint.limits.max,
                        value =>
                        {
                            var limits = sliderJoint.limits;
                            limits.max = value;
                            sliderJoint.limits = limits;
                        }
                    ));
                    break;

                case JointType.Wheel:
                    var wheelJoint = (WheelJoint2D)joint;
                    matrix.AddPort(new BsPort(Id, "damping-ratio",
                        () => wheelJoint.suspension.dampingRatio,
                        value =>
                        {
                            var suspension = wheelJoint.suspension;
                            suspension.dampingRatio = value;
                            wheelJoint.suspension = suspension;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "damping-frequency",
                        () => wheelJoint.suspension.frequency,
                        value =>
                        {
                            var suspension = wheelJoint.suspension;
                            suspension.frequency = value;
                            wheelJoint.suspension = suspension;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "angle-value",
                        () => wheelJoint.suspension.angle,
                        value =>
                        {
                            var suspension = wheelJoint.suspension;
                            suspension.angle = value;
                            wheelJoint.suspension = suspension;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-enabled",
                        () => BsMatrix.Bool2Logic(wheelJoint.useMotor),
                        value => wheelJoint.useMotor = BsMatrix.Logic2Bool(value)
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-speed",
                        () => wheelJoint.motor.motorSpeed,
                        value =>
                        {
                            var motor = wheelJoint.motor;
                            motor.motorSpeed = value;
                            wheelJoint.motor = motor;
                        }
                    ));
                    matrix.AddPort(new BsPort(Id, "motor-force",
                        () => wheelJoint.motor.maxMotorTorque,
                        value =>
                        {
                            var motor = wheelJoint.motor;
                            motor.maxMotorTorque = value;
                            wheelJoint.motor = motor;
                        }
                    ));
                    break;

                default:
                    throw Errors.InvalidJointType(JointType);
            }
        }

        protected override string[] _ToLcs()
        {
            var result = new List<string>
            {
                LcsParser.Stringify(Transform),
                LcsParser.Stringify(JointType),
                LcsParser.Stringify(MountShape),
                LcsParser.Stringify(SelfCollision),
                LcsParser.Stringify(BreakForce),
                LcsParser.Stringify(BreakTorque)
            };
            switch (JointType)
            {
                case JointType.Fixed:
                    result.Add(LcsParser.Stringify(DampingRatio));
                    result.Add(LcsParser.Stringify(DampingFrequency));
                    break;
                case JointType.Distance:
                    result.Add(LcsParser.Stringify(DistanceValue));
                    result.Add(LcsParser.Stringify(DistanceAuto));
                    result.Add(LcsParser.Stringify(DistanceMax));
                    break;
                case JointType.Spring:
                    result.Add(LcsParser.Stringify(DistanceValue));
                    result.Add(LcsParser.Stringify(DistanceAuto));
                    result.Add(LcsParser.Stringify(DampingRatio));
                    result.Add(LcsParser.Stringify(DampingFrequency));
                    break;
                case JointType.Hinge:
                    result.Add(LcsParser.Stringify(MotorEnabled));
                    result.Add(LcsParser.Stringify(MotorSpeed));
                    result.Add(LcsParser.Stringify(MotorForce));
                    result.Add(LcsParser.Stringify(LimitEnabled));
                    result.Add(LcsParser.Stringify(LimitMin));
                    result.Add(LcsParser.Stringify(LimitMax));
                    break;
                case JointType.Slider:
                    result.Add(LcsParser.Stringify(AngleValue));
                    result.Add(LcsParser.Stringify(AngleAuto));
                    result.Add(LcsParser.Stringify(MotorEnabled));
                    result.Add(LcsParser.Stringify(MotorSpeed));
                    result.Add(LcsParser.Stringify(MotorForce));
                    result.Add(LcsParser.Stringify(LimitEnabled));
                    result.Add(LcsParser.Stringify(LimitMin));
                    result.Add(LcsParser.Stringify(LimitMax));
                    break;
                case JointType.Wheel:
                    result.Add(LcsParser.Stringify(DampingRatio));
                    result.Add(LcsParser.Stringify(DampingFrequency));
                    result.Add(LcsParser.Stringify(AngleValue));
                    result.Add(LcsParser.Stringify(AngleAuto));
                    result.Add(LcsParser.Stringify(MotorEnabled));
                    result.Add(LcsParser.Stringify(MotorSpeed));
                    result.Add(LcsParser.Stringify(MotorForce));
                    break;
                default:
                    throw Errors.InvalidJointType(JointType);
            }
            return result.ToArray();
        }

        protected override void _FromLcs(string[] properties)
        {
            Transform = LcsParser.ParseTransform(properties[0]);
            JointType = LcsParser.ParseJointType(properties[1]);
            MountShape = LcsParser.ParseString(properties[2]);
            SelfCollision = LcsParser.ParseBool(properties[3]);
            BreakForce = LcsParser.ParseFloat(properties[4]);
            BreakTorque = LcsParser.ParseFloat(properties[5]);
            switch (JointType)
            {
                case JointType.Fixed:
                    DampingRatio = LcsParser.ParseFloat(properties[6]);
                    DampingFrequency = LcsParser.ParseFloat(properties[7]);
                    break;
                case JointType.Distance:
                    DistanceValue = LcsParser.ParseFloat(properties[6]);
                    DistanceAuto = LcsParser.ParseBool(properties[7]);
                    DistanceMax = LcsParser.ParseBool(properties[8]);
                    break;
                case JointType.Spring:
                    DistanceValue = LcsParser.ParseFloat(properties[6]);
                    DistanceAuto = LcsParser.ParseBool(properties[7]);
                    DampingRatio = LcsParser.ParseFloat(properties[8]);
                    DampingFrequency = LcsParser.ParseFloat(properties[9]);
                    break;
                case JointType.Hinge:
                    MotorEnabled = LcsParser.ParseBool(properties[6]);
                    MotorSpeed = LcsParser.ParseFloat(properties[7]);
                    MotorForce = LcsParser.ParseFloat(properties[8]);
                    LimitEnabled = LcsParser.ParseBool(properties[9]);
                    LimitMin = LcsParser.ParseFloat(properties[10]);
                    LimitMax = LcsParser.ParseFloat(properties[11]);
                    break;
                case JointType.Slider:
                    AngleValue = LcsParser.ParseFloat(properties[6]);
                    AngleAuto = LcsParser.ParseBool(properties[7]);
                    MotorEnabled = LcsParser.ParseBool(properties[8]);
                    MotorSpeed = LcsParser.ParseFloat(properties[9]);
                    MotorForce = LcsParser.ParseFloat(properties[10]);
                    LimitEnabled = LcsParser.ParseBool(properties[11]);
                    LimitMin = LcsParser.ParseFloat(properties[12]);
                    LimitMax = LcsParser.ParseFloat(properties[13]);
                    break;
                case JointType.Wheel:
                    DampingRatio = LcsParser.ParseFloat(properties[6]);
                    DampingFrequency = LcsParser.ParseFloat(properties[7]);
                    AngleValue = LcsParser.ParseFloat(properties[8]);
                    AngleAuto = LcsParser.ParseBool(properties[9]);
                    MotorEnabled = LcsParser.ParseBool(properties[10]);
                    MotorSpeed = LcsParser.ParseFloat(properties[11]);
                    MotorForce = LcsParser.ParseFloat(properties[12]);
                    break;
                default:
                    throw Errors.InvalidJointType(JointType);
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
