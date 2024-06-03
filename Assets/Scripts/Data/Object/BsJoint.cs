using System;
using Controllers;
using Controllers.Base;
using Data.Base;
using Extensions;
using Systems;
using UnityEngine;
using Utils;

namespace Data.Object
{
    public class BsJoint : BsObject
    {
        public const byte TypeFixed = 0;
        public const byte TypeDistance = 1;
        public const byte TypeSpring = 2;
        public const byte TypeHinge = 3;
        public const byte TypeSlider = 4;

        public override GameObject Prefab => ResourceSystem._.pJoint;
        public override string Tag => Tags.JointPrefix;

        public override Func<BsBehavior, BsNode> GetNode => controller =>
        {
            var joint = controller.transform.parent.GetComponent<AnchoredJoint2D>();
            switch (Type)
            {
                case TypeFixed:
                    var fixedJoint = (FixedJoint2D)joint;
                    return new BsNode(Tag)
                    {
                        GetPorts = () => new[]
                        {
                            new BsPort("frequency", BsPort.TypeFloat,
                                (_, value) => fixedJoint.frequency = (float)value),
                            new BsPort("damping", BsPort.TypeFloat,
                                (_, value) => fixedJoint.dampingRatio = (float)value)
                        }
                    };

                case TypeDistance:
                    var distanceJoint = (DistanceJoint2D)joint;
                    return new BsNode(Tag)
                    {
                        GetPorts = () => new[]
                        {
                            new BsPort("frequency", BsPort.TypeFloat,
                                (_, value) => distanceJoint.distance = (float)value),
                            new BsPort("distance-auto", BsPort.TypeBool,
                                (_, value) => distanceJoint.autoConfigureDistance = (bool)value),
                            new BsPort("distance-max", BsPort.TypeBool,
                                (_, value) => distanceJoint.maxDistanceOnly = (bool)value)
                        }
                    };

                case TypeSpring:
                    var springJoint = (SpringJoint2D)joint;
                    return new BsNode(Tag)
                    {
                        GetPorts = () => new[]
                        {
                            new BsPort("frequency", BsPort.TypeFloat,
                                (_, value) => springJoint.frequency = (float)value),
                            new BsPort("damping", BsPort.TypeFloat,
                                (_, value) => springJoint.dampingRatio = (float)value),
                            new BsPort("frequency", BsPort.TypeFloat,
                                (_, value) => springJoint.distance = (float)value),
                            new BsPort("distance-auto", BsPort.TypeBool,
                                (_, value) => springJoint.autoConfigureDistance = (bool)value)
                        }
                    };

                case TypeHinge:
                    var hingeJoint = (HingeJoint2D)joint;
                    return new BsNode(Tag)
                    {
                        GetPorts = () => new[]
                        {
                            new BsPort("motor", BsPort.TypeBool, (_, value) => hingeJoint.useMotor = (bool)value),
                            new BsPort("motor-speed", BsPort.TypeFloat, (_, value) =>
                            {
                                var motor = hingeJoint.motor;
                                motor.motorSpeed = (float)value;
                                hingeJoint.motor = motor;
                            }),
                            new BsPort("motor-force", BsPort.TypeFloat, (_, value) =>
                            {
                                var motor = hingeJoint.motor;
                                motor.maxMotorTorque = (float)value;
                                hingeJoint.motor = motor;
                            }),
                            new BsPort("limit", BsPort.TypeBool, (_, value) => hingeJoint.useLimits = (bool)value),
                            new BsPort("limit-min", BsPort.TypeFloat, (_, value) =>
                            {
                                var limits = hingeJoint.limits;
                                limits.min = (float)value;
                                hingeJoint.limits = limits;
                            }),
                            new BsPort("limit-max", BsPort.TypeFloat, (_, value) =>
                            {
                                var limits = hingeJoint.limits;
                                limits.max = (float)value;
                                hingeJoint.limits = limits;
                            })
                        }
                    };

                case TypeSlider:
                    var sliderJoint = (SliderJoint2D)joint;
                    return new BsNode(Tag)
                    {
                        GetPorts = () => new[]
                        {
                            new BsPort("angle", BsPort.TypeFloat, (_, value) => sliderJoint.angle = (float)value),
                            new BsPort("angle-auto", BsPort.TypeBool,
                                (_, value) => sliderJoint.autoConfigureAngle = (bool)value),
                            new BsPort("motor", BsPort.TypeBool, (_, value) => sliderJoint.useMotor = (bool)value),
                            new BsPort("motor-speed", BsPort.TypeFloat, (_, value) =>
                            {
                                var motor = sliderJoint.motor;
                                motor.motorSpeed = (float)value;
                                sliderJoint.motor = motor;
                            }),
                            new BsPort("motor-force", BsPort.TypeFloat, (_, value) =>
                            {
                                var motor = sliderJoint.motor;
                                motor.maxMotorTorque = (float)value;
                                sliderJoint.motor = motor;
                            }),
                            new BsPort("limit", BsPort.TypeBool, (_, value) => sliderJoint.useLimits = (bool)value),
                            new BsPort("limit-min", BsPort.TypeFloat, (_, value) =>
                            {
                                var limits = sliderJoint.limits;
                                limits.min = (float)value;
                                sliderJoint.limits = limits;
                            }),
                            new BsPort("limit-max", BsPort.TypeFloat, (_, value) =>
                            {
                                var limits = sliderJoint.limits;
                                limits.max = (float)value;
                                sliderJoint.limits = limits;
                            })
                        }
                    };

                default:
                    throw Errors.InvalidItem("joint type", Type);
            }
        };

        public override Func<GameObject, BsObject, BsObject[], BsBehavior> Init => (gob, obj, rel) =>
        {
            // Link object to controller
            var joint = obj.As<BsJoint>();
            var controller = gob.GetComponent<JointController>();
            controller.Object = joint;

            // Create joint component
            if (rel.Length == 0) throw Errors.NoItemFound("joint parent shape", "");
            var parentGameObject = rel[0].InstanceObject;
            if (!parentGameObject) throw Errors.NoItemFound("joint parent shape", rel[0].Id);
            AnchoredJoint2D component = joint.Type switch
            {
                TypeFixed => parentGameObject.AddComponent<FixedJoint2D>(),
                TypeDistance => parentGameObject.AddComponent<DistanceJoint2D>(),
                TypeSpring => parentGameObject.AddComponent<SpringJoint2D>(),
                TypeHinge => parentGameObject.AddComponent<HingeJoint2D>(),
                TypeSlider => parentGameObject.AddComponent<SliderJoint2D>(),
                _ => throw Errors.InvalidItem("joint type", joint.Type)
            };

            // Apply universal joint config
            component.anchor = joint.SelfAnchor;
            component.breakForce = joint.BreakForce;
            component.breakTorque = joint.BreakTorque;

            // Apply specific joint config
            switch (joint.Type)
            {
                case TypeFixed:
                    var fixedJointComponent = (FixedJoint2D)component;
                    fixedJointComponent.dampingRatio = joint.Damping;
                    fixedJointComponent.frequency = joint.Frequency;
                    break;

                case TypeDistance:
                    var distanceJointComponent = (DistanceJoint2D)component;
                    distanceJointComponent.distance = joint.Distance;
                    distanceJointComponent.autoConfigureDistance = joint.DistanceAuto;
                    distanceJointComponent.maxDistanceOnly = joint.DistanceMax;
                    break;

                case TypeSpring:
                    var springJointComponent = (SpringJoint2D)component;
                    springJointComponent.distance = joint.Distance;
                    springJointComponent.autoConfigureDistance = joint.DistanceAuto;
                    springJointComponent.dampingRatio = joint.Damping;
                    springJointComponent.frequency = joint.Frequency;
                    break;

                case TypeHinge:
                    var hingeJointComponent = (HingeJoint2D)component;
                    if (joint.Motor)
                    {
                        hingeJointComponent.useMotor = true;
                        var hingeJointMotor = hingeJointComponent.motor;
                        hingeJointMotor.motorSpeed = joint.MotorSpeed;
                        hingeJointMotor.maxMotorTorque = joint.MotorForce;
                        hingeJointComponent.motor = hingeJointMotor;
                    }
                    if (joint.Limit)
                    {
                        hingeJointComponent.useLimits = true;
                        var hingeJointLimits = hingeJointComponent.limits;
                        hingeJointLimits.min = joint.LimitMin;
                        hingeJointLimits.max = joint.LimitMax;
                        hingeJointComponent.limits = hingeJointLimits;
                    }
                    break;

                case TypeSlider:
                    var sliderJointComponent = (SliderJoint2D)component;
                    sliderJointComponent.angle = joint.Angle;
                    sliderJointComponent.autoConfigureAngle = joint.AngleAuto;
                    if (joint.Motor)
                    {
                        sliderJointComponent.useMotor = true;
                        var sliderJointMotor = sliderJointComponent.motor;
                        sliderJointMotor.motorSpeed = joint.MotorSpeed;
                        sliderJointMotor.maxMotorTorque = joint.MotorForce;
                        sliderJointComponent.motor = sliderJointMotor;
                    }
                    if (joint.Limit)
                    {
                        sliderJointComponent.useLimits = true;
                        var sliderJointLimits = sliderJointComponent.limits;
                        sliderJointLimits.min = joint.LimitMin;
                        sliderJointLimits.max = joint.LimitMax;
                        sliderJointComponent.limits = sliderJointLimits;
                    }
                    break;

                default:
                    throw Errors.InvalidItem("joint type", joint.Type);
            }

            // Set up connected rigidbody
            component.autoConfigureConnectedAnchor = false;
            if (rel.Length > 1)
            {
                component.enableCollision = joint.OtherCollision;
                var mountShape = rel[1].As<BsShape>();
                if (!mountShape.InstanceObject) throw Errors.ParentObjectUnbuilt();
                component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
                component.connectedAnchor = joint.OtherAnchor;
            }

            component.connectedAnchor = joint.OtherAnchor;

            return controller;
        };

        public byte Type
        {
            get => (byte)Props[0];
            set => Props[0] = value;
        }

        public Vector2 SelfAnchor
        {
            get => Vector2Ext.FromLcs(Props[1]);
            set => Props[1] = value.ToLcs();
        }

        public Vector2 OtherAnchor
        {
            get => Vector2Ext.FromLcs(Props[2]);
            set => Props[2] = value.ToLcs();
        }

        public bool OtherCollision // Universal
        {
            get => (bool)Props[3];
            set => Props[3] = value;
        }

        public float BreakForce // Universal
        {
            get => (float)Props[4];
            set => Props[4] = value;
        }

        public float BreakTorque // Universal
        {
            get => (float)Props[5];
            set => Props[5] = value;
        }

        public float Angle // Slider, Wheel
        {
            get => (float)Props[6];
            set => Props[6] = value;
        }

        public bool AngleAuto // Slider, Wheel
        {
            get => (bool)Props[7];
            set => Props[7] = value;
        }

        public float Distance // Distance, Spring
        {
            get => (float)Props[8];
            set => Props[8] = value;
        }

        public bool DistanceAuto // Distance, Spring
        {
            get => (bool)Props[9];
            set => Props[9] = value;
        }

        public bool DistanceMax // Distance
        {
            get => (bool)Props[10];
            set => Props[10] = value;
        }

        public float Frequency // Fixed, Spring, Wheel
        {
            get => (float)Props[11];
            set => Props[11] = value;
        }

        public float Damping // Fixed, Spring, Wheel
        {
            get => (float)Props[12];
            set => Props[12] = value;
        }

        public bool Motor // Hinge, Slider, Wheel
        {
            get => (bool)Props[13];
            set => Props[13] = value;
        }

        public float MotorSpeed // Hinge, Slider, Wheel
        {
            get => (float)Props[14];
            set => Props[14] = value;
        }

        public float MotorForce // Hinge, Slider, Wheel
        {
            get => (float)Props[15];
            set => Props[15] = value;
        }

        public bool Limit // Hinge, Slider
        {
            get => (bool)Props[16];
            set => Props[16] = value;
        }

        public float LimitMin // Hinge, Slider
        {
            get => (float)Props[17];
            set => Props[17] = value;
        }

        public float LimitMax // Hinge, Slider
        {
            get => (float)Props[18];
            set => Props[18] = value;
        }

        public BsJoint(string id = "", params string[] relatives) : base(id, relatives)
        {
            Props = new[]
            {
                TypeFixed, Vector2.zero.ToLcs(), Vector2.zero.ToLcs(), false, float.PositiveInfinity,
                float.PositiveInfinity, 0f, false, 0f, false, false, .2f, 2f, false, 0f, 0f, false, 0f, 0f
            };
        }

        public BsJoint() { }
    }
}
