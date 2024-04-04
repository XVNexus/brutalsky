using System.Collections.Generic;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Utils.Constants;
using Utils.Ext;

namespace Serializable
{
    public class SrzJoint
    {
        public BsJointType jt { get; set; }
        public string tr { get; set; }
        public string ts { get; set; }
        public string ms { get; set; }
        public Dictionary<string, string> pr { get; set; }

        public static SrzJoint Simplify(BsJoint joint)
        {
            var properties = new Dictionary<string, string>
            {
                ["cl"] = BoolExt.ToString(joint.Collision),
                ["st"] = joint.Strength.ToString()
            };
            switch (joint.JointType)
            {
                case BsJointType.Fixed:
                    var fixedJoint = (BsJointFixed)joint;
                    properties["dm"] = fixedJoint.Damping.ToString();
                    break;
                case BsJointType.Distance:
                    var distanceJoint = (BsJointDistance)joint;
                    properties["ds"] = distanceJoint.Distance.ToString();
                    properties["md"] = BoolExt.ToString(distanceJoint.MaxDistanceOnly);
                    break;
                case BsJointType.Spring:
                    var springJoint = (BsJointSpring)joint;
                    properties["ds"] = springJoint.Distance.ToString();
                    properties["dm"] = springJoint.Damping.ToString();
                    break;
                case BsJointType.Hinge:
                    var hingeJoint = (BsJointHinge)joint;
                    properties["mt"] = hingeJoint.Motor.ToString();
                    properties["lm"] = hingeJoint.Limits.ToString();
                    break;
                case BsJointType.Slider:
                    var sliderJoint = (BsJointSlider)joint;
                    properties["an"] = sliderJoint.Angle.ToString();
                    properties["mt"] = sliderJoint.Motor.ToString();
                    properties["lm"] = sliderJoint.Limits.ToString();
                    break;
                case BsJointType.Wheel:
                    var wheelJoint = (BsJointWheel)joint;
                    properties["sd"] = wheelJoint.SuspensionDamping.ToString();
                    properties["sa"] = wheelJoint.SuspensionAngle.ToString();
                    properties["mt"] = wheelJoint.Motor.ToString();
                    break;
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(joint.JointType);
            }
            return new SrzJoint
            {
                jt = joint.JointType,
                tr = joint.Transform.ToString(),
                ts = joint.TargetShapeId,
                ms = joint.MountShapeId,
                pr = properties
            };
        }

        public BsJoint Expand(string id)
        {
            var transformParsed = BsTransform.Parse(tr);
            var collision = BoolExt.Parse(pr["cl"]);
            var strength = BsJointStrength.Parse(pr["st"]);
            switch (jt)
            {
                case BsJointType.Fixed:
                    return new BsJointFixed
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointDamping.Parse(pr["dm"])
                    );
                case BsJointType.Distance:
                    return new BsJointDistance
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointConfig.Parse(pr["ds"]),
                        BoolExt.Parse(pr["md"])
                    );
                case BsJointType.Spring:
                    return new BsJointSpring
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointConfig.Parse(pr["ds"]),
                        BsJointDamping.Parse(pr["dm"])
                    );
                case BsJointType.Hinge:
                    return new BsJointHinge
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointMotor.Parse(pr["mt"]),
                        BsJointLimits.Parse(pr["lm"])
                    );
                case BsJointType.Slider:
                    return new BsJointSlider
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointConfig.Parse(pr["an"]),
                        BsJointMotor.Parse(pr["mt"]),
                        BsJointLimits.Parse(pr["lm"])
                    );
                case BsJointType.Wheel:
                    return new BsJointWheel
                    (
                        id, transformParsed, ts, ms, collision, strength,
                        BsJointDamping.Parse(pr["sd"]),
                        float.Parse(pr["sa"]),
                        BsJointMotor.Parse(pr["mt"])
                    );
                case BsJointType.None:
                default:
                    throw Errors.InvalidJointType(jt);
            }
        }
    }
}
