using System.Collections.Generic;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Utils;
using Utils.Ext;

namespace Serializable
{
    public class SrzJoint
    {
        public BsJointType jointType { get; set; }
        public string id { get; set; }
        public string transform { get; set; }
        public string targetShape { get; set; }
        public string mountShape { get; set; }
        public Dictionary<string, string> properties { get; set; }

        public static SrzJoint Simplify(BsJoint joint)
        {
            var properties = new Dictionary<string, string>
            {
                ["collision"] = BoolExt.ToString(joint.Collision),
                ["strength"] = joint.Strength.ToString()
            };
            switch (joint.JointType)
            {
                case BsJointType.Fixed:
                    var fixedJoint = (BsJointFixed)joint;
                    properties["damping"] = fixedJoint.Damping.ToString();
                    break;
                case BsJointType.Distance:
                    var distanceJoint = (BsJointDistance)joint;
                    properties["distance"] = distanceJoint.Distance.ToString();
                    properties["maxDistanceOnly"] = BoolExt.ToString(distanceJoint.MaxDistanceOnly);
                    break;
                case BsJointType.Spring:
                    var springJoint = (BsJointSpring)joint;
                    properties["distance"] = springJoint.Distance.ToString();
                    properties["damping"] = springJoint.Damping.ToString();
                    break;
                case BsJointType.Hinge:
                    var hingeJoint = (BsJointHinge)joint;
                    properties["motor"] = hingeJoint.Motor.ToString();
                    properties["limits"] = hingeJoint.Limits.ToString();
                    break;
                case BsJointType.Slider:
                    var sliderJoint = (BsJointSlider)joint;
                    properties["angle"] = sliderJoint.Angle.ToString();
                    properties["motor"] = sliderJoint.Motor.ToString();
                    properties["limits"] = sliderJoint.Limits.ToString();
                    break;
                case BsJointType.Wheel:
                    var wheelJoint = (BsJointWheel)joint;
                    properties["suspensionDamping"] = wheelJoint.SuspensionDamping.ToString();
                    properties["suspensionAngle"] = wheelJoint.SuspensionAngle.ToString();
                    properties["motor"] = wheelJoint.Motor.ToString();
                    break;
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(joint.JointType);
            }
            return new SrzJoint
            {
                jointType = joint.JointType,
                id = joint.Id,
                transform = joint.Transform.ToString(),
                targetShape = joint.TargetShapeId,
                mountShape = joint.MountShapeId,
                properties = properties
            };
        }

        public BsJoint Expand()
        {
            var transformParsed = BsTransform.Parse(transform);
            var collision = BoolExt.Parse(properties["collision"]);
            var strength = BsJointStrength.Parse(properties["strength"]);
            switch (jointType)
            {
                case BsJointType.Fixed:
                    return new BsJointFixed
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointDamping.Parse(properties["damping"])
                    );
                case BsJointType.Distance:
                    return new BsJointDistance
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointConfig.Parse(properties["distance"]),
                        BoolExt.Parse(properties["maxDistanceOnly"])
                    );
                case BsJointType.Spring:
                    return new BsJointSpring
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointConfig.Parse(properties["distance"]),
                        BsJointDamping.Parse(properties["damping"])
                    );
                case BsJointType.Hinge:
                    return new BsJointHinge
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointMotor.Parse(properties["motor"]),
                        BsJointLimits.Parse(properties["limits"])
                    );
                case BsJointType.Slider:
                    return new BsJointSlider
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointConfig.Parse(properties["angle"]),
                        BsJointMotor.Parse(properties["motor"]),
                        BsJointLimits.Parse(properties["limits"])
                    );
                case BsJointType.Wheel:
                    return new BsJointWheel
                    (
                        id, transformParsed, targetShape, mountShape, collision, strength,
                        BsJointDamping.Parse(properties["suspensionDamping"]),
                        float.Parse(properties["suspensionAngle"]),
                        BsJointMotor.Parse(properties["motor"])
                    );
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(jointType);
            }
        }
    }
}
