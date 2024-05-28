using Controllers;
using Controllers.Base;
using Data.Base;
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
        public const byte TypeWheel = 5;

        public override GameObject Prefab => ResourceSystem._.pJoint;
        public override string Tag => Tags.MountPrefix;

        public byte Type { get; set; }
        public Vector2 SelfAnchor { get; set; } = Vector2.zero;
        public Vector2 OtherAnchor { get; set; } = Vector2.zero;
        public bool OtherCollision { get; set; } // Universal

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

        public BsJoint(string id = "", params string[] relatives) : base(id, relatives) { }

        protected override BsBehavior _Init(GameObject gameObject, BsObject[] relatedObjects)
        {
            // Link object to controller
            var controller = gameObject.GetComponent<JointController>();
            controller.Object = this;

            // Create joint component
            if (relatedObjects.Length == 0) throw Errors.NoItemFound("joint parent shape", "");
            var parentGameObject = relatedObjects[0].InstanceObject;
            if (!parentGameObject) throw Errors.NoItemFound("joint parent shape", relatedObjects[0].Id);
            AnchoredJoint2D component = Type switch
            {
                TypeFixed => parentGameObject.AddComponent<FixedJoint2D>(),
                TypeDistance => parentGameObject.AddComponent<DistanceJoint2D>(),
                TypeSpring => parentGameObject.AddComponent<SpringJoint2D>(),
                TypeHinge => parentGameObject.AddComponent<HingeJoint2D>(),
                TypeSlider => parentGameObject.AddComponent<SliderJoint2D>(),
                TypeWheel => parentGameObject.AddComponent<WheelJoint2D>(),
                _ => throw Errors.InvalidItem("joint type", Type)
            };

            // Apply universal joint config
            component.anchor = SelfAnchor;
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
            if (relatedObjects.Length > 1)
            {
                component.enableCollision = OtherCollision;
                var mountShape = (BsShape)relatedObjects[1];
                if (!mountShape.InstanceObject) throw Errors.ParentObjectUnbuilt();
                component.connectedBody = mountShape.InstanceObject.GetComponent<Rigidbody2D>();
                component.connectedAnchor = OtherAnchor;
            }
            component.connectedAnchor = OtherAnchor;

            return controller;
        }
    }
}
