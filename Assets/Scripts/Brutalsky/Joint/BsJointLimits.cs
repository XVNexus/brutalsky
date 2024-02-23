namespace Brutalsky.Joint
{
    public struct BsJointLimits
    {
        public bool use { get; set; }
        public float min { get; set; }
        public float max { get; set; }

        private BsJointLimits(bool use, float min, float max)
        {
            this.use = use;
            this.min = min;
            this.max = max;
        }

        public static BsJointLimits Limited(float min, float max)
        {
            return new BsJointLimits(true, min, max);
        }

        public static BsJointLimits Unlimited()
        {
            return new BsJointLimits(false, 0f, 0f);
        }

        public static BsJointLimits Parse(string raw)
        {
            if (raw == "x") return Unlimited();
            var parts = raw.Split(' ');
            return Limited(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return use ? $"{min} {max}" : "x";
        }
    }
}
