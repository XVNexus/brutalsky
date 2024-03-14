namespace Brutalsky.Joint
{
    public struct BsJointDamping
    {
        public float Ratio { get; set; }
        public float Frequency { get; set; }

        private BsJointDamping(float ratio, float frequency)
        {
            Ratio = ratio;
            Frequency = frequency;
        }

        public static BsJointDamping Damped(float ratio, float frequency = 1f) => new(ratio, frequency);

        public static BsJointDamping Free(float frequency = 1f) => new(0f, frequency);

        public static BsJointDamping Parse(string raw)
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
