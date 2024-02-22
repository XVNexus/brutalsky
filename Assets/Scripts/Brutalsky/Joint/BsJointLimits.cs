namespace Brutalsky.Joint
{
    public struct BsJointLimits
    {
        public bool use { get; set; }
        public float min { get; set; }
        public float max { get; set; }

        public BsJointLimits(bool use, float min, float max)
        {
            this.use = use;
            this.min = min;
            this.max = max;
        }
    }
}
