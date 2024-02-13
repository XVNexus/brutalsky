using JetBrains.Annotations;

namespace Brutalsky
{
    public class BsJoint : BsObject
    {
        public BsJointType jointType { get; set; }
        [CanBeNull] public BsShape mountShape { get; set; }
        public BsShape targetShape { get; set; }

        public BsJoint(BsTransform transform, BsJointType jointType, [CanBeNull] BsShape mountShape, BsShape targetShape)
        {
            this.transform = transform;
            this.jointType = jointType;
            this.mountShape = mountShape;
            this.targetShape = targetShape;
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
