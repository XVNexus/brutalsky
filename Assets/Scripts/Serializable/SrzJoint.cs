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
        public string targetShapeId { get; set; }
        public string mountShapeId { get; set; }
        public Dictionary<string, string> properties { get; set; }

        public static SrzJoint Simplify(BsJoint joint)
        {
            var properties = new Dictionary<string, string>
            {
                ["collision"] = BoolExt.ToString(joint.collision),
                ["strength"] = joint.strength.ToString()
            };
            switch (joint.jointType)
            {
                case BsJointType.Fixed:
                    var fixedJoint = (BsJointFixed)joint;
                    properties["damping"] = fixedJoint.damping.ToString();
                    break;
                case BsJointType.Distance:
                    var distanceJoint = (BsJointDistance)joint;
                    properties["distance"] = distanceJoint.distance.ToString();
                    properties["maxDistanceOnly"] = BoolExt.ToString(distanceJoint.maxDistanceOnly);
                    break;
                case BsJointType.Spring:
                    var springJoint = (BsJointSpring)joint;
                    properties["distance"] = springJoint.distance.ToString();
                    properties["damping"] = springJoint.damping.ToString();
                    break;
                case BsJointType.Hinge:
                    var hingeJoint = (BsJointHinge)joint;
                    properties["motor"] = hingeJoint.motor.ToString();
                    properties["limits"] = hingeJoint.limits.ToString();
                    break;
                case BsJointType.Slider:
                    var sliderJoint = (BsJointSlider)joint;
                    properties["angle"] = sliderJoint.angle.ToString();
                    properties["motor"] = sliderJoint.motor.ToString();
                    properties["limits"] = sliderJoint.limits.ToString();
                    break;
                case BsJointType.Wheel:
                    var wheelJoint = (BsJointWheel)joint;
                    properties["suspensionDamping"] = wheelJoint.suspensionDamping.ToString();
                    properties["suspensionAngle"] = wheelJoint.suspensionAngle.ToString();
                    properties["motor"] = wheelJoint.motor.ToString();
                    break;
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(joint.jointType);
            }
            return new SrzJoint
            {
                jointType = joint.jointType,
                id = joint.id,
                transform = joint.transform.ToString(),
                targetShapeId = joint.targetShapeId,
                mountShapeId = joint.mountShapeId,
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
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
                        BsJointDamping.Parse(properties["damping"])
                    );
                case BsJointType.Distance:
                    return new BsJointDistance
                    (
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
                        BsJointConfig.Parse(properties["distance"]),
                        BoolExt.Parse(properties["maxDistanceOnly"])
                    );
                case BsJointType.Spring:
                    return new BsJointSpring
                    (
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
                        BsJointConfig.Parse(properties["distance"]),
                        BsJointDamping.Parse(properties["damping"])
                    );
                case BsJointType.Hinge:
                    return new BsJointHinge
                    (
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
                        BsJointMotor.Parse(properties["motor"]),
                        BsJointLimits.Parse(properties["limits"])
                    );
                case BsJointType.Slider:
                    return new BsJointSlider
                    (
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
                        BsJointConfig.Parse(properties["angle"]),
                        BsJointMotor.Parse(properties["motor"]),
                        BsJointLimits.Parse(properties["limits"])
                    );
                case BsJointType.Wheel:
                    return new BsJointWheel
                    (
                        id, transformParsed, targetShapeId, mountShapeId, collision, strength,
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
