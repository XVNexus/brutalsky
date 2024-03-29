namespace Brutalsky.Joint
{
    public struct BsJointLimits
    {
        public bool Use { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }

        private BsJointLimits(bool use, float min, float max)
        {
            Use = use;
            Min = min;
            Max = max;
        }

        public static BsJointLimits Limited(float min, float max) => new(true, min, max);

        public static BsJointLimits Unlimited() => new(false, 0f, 0f);

        public static BsJointLimits Parse(string raw)
        {
            if (raw == "x") return Unlimited();
            var parts = raw.Split(' ');
            return Limited(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return Use ? $"{Min} {Max}" : "x";
        }
    }
}
