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
    }
}
