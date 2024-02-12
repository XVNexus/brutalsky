using UnityEngine;

namespace Brutalsky
{
    public class BsJoint : BsObject
    {
        public BsJointType jointType { get; set; }
        public BsShape mountShape { get; set; }
        public BsShape targetShape { get; set; }

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
