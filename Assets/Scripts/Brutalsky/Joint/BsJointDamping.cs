namespace Brutalsky.Joint
{
    public struct BsJointDamping
    {
        public float ratio { get; set; }
        public float frequency { get; set; }

        public BsJointDamping(float ratio, float frequency)
        {
            this.ratio = ratio;
            this.frequency = frequency;
        }
    }
}
