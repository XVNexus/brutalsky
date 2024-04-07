namespace Utils.Joint
{
    public struct JointLimits
    {
        public bool Use { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }

        private JointLimits(bool use, float min, float max)
        {
            Use = use;
            Min = min;
            Max = max;
        }

        public static JointLimits Limited(float min, float max) => new(true, min, max);

        public static JointLimits Unlimited() => new(false, 0f, 0f);
    }
}
