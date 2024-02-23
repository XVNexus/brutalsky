namespace Brutalsky.Joint
{
    public struct BsJointStrength
    {
        public float breakForce { get; set; }
        public float breakTorque { get; set; }

        private BsJointStrength(float breakForce, float breakTorque)
        {
            this.breakForce = breakForce;
            this.breakTorque = breakTorque;
        }

        public static BsJointStrength Breakable(float breakForce, float breakTorque = float.PositiveInfinity)
        {
            return new BsJointStrength(breakForce, breakTorque);
        }

        public static BsJointStrength Immortal()
        {
            return new BsJointStrength(float.PositiveInfinity, float.PositiveInfinity);
        }
    }
}
