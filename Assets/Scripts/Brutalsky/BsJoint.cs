using UnityEngine;

namespace Brutalsky
{
    public class BsJoint : BsObject
    {
        public BsJointType jointType { get; set; }
        public BsShape mountShape { get; set; }
        public BsShape targetShape { get; set; }

        public BsJoint(string id, BsJointType jointType, BsShape mountShape, BsShape targetShape) : base(id)
        {
            this.jointType = jointType;
            this.mountShape = mountShape;
            this.targetShape = targetShape;
        }

        protected override GameObject _Create()
        {
            throw new System.NotImplementedException();
        }
    }

    public enum BsJointType
    {
        Distance,
        Fixed,
        Friction,
        Hinge,
        Relative,
        Slider,
        Spring,
        Target,
        Wheel
    }
}
