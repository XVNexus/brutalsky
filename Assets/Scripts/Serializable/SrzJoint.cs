using System.Collections.Generic;
using Brutalsky;
using Utils.Constants;
using Utils.Ext;
using Utils.Joint;
using Utils.Object;

namespace Serializable
{
    public class SrzJoint
    {
        public JointType jt { get; set; }
        public string tr { get; set; }
        public string ts { get; set; }
        public string ms { get; set; }
        public Dictionary<string, string> pr { get; set; }

        public static SrzJoint Simplify(BsJoint joint)
        {
            var properties = new Dictionary<string, string>
            {
                ["cl"] = BoolExt.Stringify(joint.Collision),
                ["st"] = joint.Strength.ToString()
            };
            switch (joint.JointType)
            {
                case JointType.Fixed:
                    properties["dm"] = joint.Damping.ToString();
                    break;
                case JointType.Distance:
                    properties["ds"] = joint.Distance.ToString();
                    properties["md"] = BoolExt.Stringify(joint.MaxDistanceOnly);
                    break;
                case JointType.Spring:
                    properties["ds"] = joint.Distance.ToString();
                    properties["dm"] = joint.Damping.ToString();
                    break;
                case JointType.Hinge:
                    properties["mt"] = joint.Motor.ToString();
                    properties["lm"] = joint.Limits.ToString();
                    break;
                case JointType.Slider:
                    properties["an"] = joint.Angle.ToString();
                    properties["mt"] = joint.Motor.ToString();
                    properties["lm"] = joint.Limits.ToString();
                    break;
                case JointType.Wheel:
                    properties["dm"] = joint.Damping.ToString();
                    properties["an"] = joint.Angle.ToString();
                    properties["mt"] = joint.Motor.ToString();
                    break;
                case JointType.None:
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
            var transformParsed = ObjectTransform.Parse(tr);
            var collision = BoolExt.Parse(pr["cl"]);
            var strength = JointStrength.Parse(pr["st"]);
            var result = new BsJoint(id, transformParsed, ts, ms, collision, strength);
            switch (jt)
            {
                case JointType.Fixed:
                    return result.FixedJoint
                    (
                        JointDamping.Parse(pr["dm"])
                    );
                case JointType.Distance:
                    return result.DistanceJoint
                    (
                        JointConfig.Parse(pr["ds"]),
                        BoolExt.Parse(pr["md"])
                    );
                case JointType.Spring:
                    return result.SpringJoint
                    (
                        JointConfig.Parse(pr["ds"]),
                        JointDamping.Parse(pr["dm"])
                    );
                case JointType.Hinge:
                    return result.HingeJoint
                    (
                        JointMotor.Parse(pr["mt"]),
                        JointLimits.Parse(pr["lm"])
                    );
                case JointType.Slider:
                    return result.SliderJoint
                    (
                        JointConfig.Parse(pr["an"]),
                        JointMotor.Parse(pr["mt"]),
                        JointLimits.Parse(pr["lm"])
                    );
                case JointType.Wheel:
                    return result.WheelJoint
                    (
                        JointDamping.Parse(pr["dm"]),
                        JointConfig.Parse(pr["an"]),
                        JointMotor.Parse(pr["mt"])
                    );
                case JointType.None:
                default:
                    throw Errors.InvalidJointType(jt);
            }
        }
    }
}
