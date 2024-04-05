namespace Utils.Joint
{
    public struct JointDamping
    {
        public float Ratio { get; set; }
        public float Frequency { get; set; }

        private JointDamping(float ratio, float frequency)
        {
            Ratio = ratio;
            Frequency = frequency;
        }

        public static JointDamping Damped(float ratio, float frequency = 1f) => new(ratio, frequency);

        public static JointDamping Free(float frequency = 1f) => new(0f, frequency);

        public static JointDamping Parse(string raw)
        {
            var parts = raw.Split(' ');
            return Damped(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return $"{Ratio} {Frequency}";
        }
    }
}
