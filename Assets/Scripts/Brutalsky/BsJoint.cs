using JetBrains.Annotations;

namespace Brutalsky
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
    }

    public enum BsJointType
    {
        Weld,
        Hinge,
        Slider,
        Spring
    }
}
