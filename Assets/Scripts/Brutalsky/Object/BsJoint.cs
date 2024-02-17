using Brutalsky.Property;
using JetBrains.Annotations;

namespace Brutalsky.Object
{
    public class BsJoint : BsObject
    {
        public BsShape targetShape { get; set; }
        [CanBeNull] public BsShape mountShape { get; set; }
        public BsJointType jointType { get; set; }
        public float speed { get; set; }
        public float torque { get; set; }
        public float strength { get; set; }

        public BsJoint(BsShape targetShape, [CanBeNull] BsShape mountShape, BsTransform transform, BsJointType jointType, float speed = 0f, float torque = 0f, float strength = 0f)
        {
            this.targetShape = targetShape;
            this.mountShape = mountShape;
            this.transform = transform;
            this.jointType = jointType;
            this.speed = speed;
            this.torque = torque;
            this.strength = strength;
        }

        public BsJoint()
        {
        }

        public override void Parse(string[] raw)
        {
        }

        public override string[] Stringify()
        {
            return new[] { "I FARTED IN UR ASS" };
        }
    }

    public enum BsJointType
    {
        Hinge,
        Slider
    }
}
