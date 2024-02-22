namespace Brutalsky.Joint
{
    public struct BsJointStrength
    {
        public float breakForce { get; set; }
        public float breakTorque { get; set; }

        public BsJointStrength(float breakForce, float breakTorque)
        {
            this.breakForce = breakForce;
            this.breakTorque = breakTorque;
        }
    }
}
