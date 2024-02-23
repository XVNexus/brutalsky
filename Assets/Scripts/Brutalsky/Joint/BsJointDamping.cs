namespace Brutalsky.Joint
{
    public struct BsJointDamping
    {
        public float ratio { get; set; }
        public float frequency { get; set; }

        private BsJointDamping(float ratio, float frequency)
        {
            this.ratio = ratio;
            this.frequency = frequency;
        }

        public static BsJointDamping Damped(float ratio, float frequency = 1f)
        {
            return new BsJointDamping(ratio, frequency);
        }

        public static BsJointDamping Free(float frequency = 1f)
        {
            return new BsJointDamping(0f, frequency);
        }
    }
}
