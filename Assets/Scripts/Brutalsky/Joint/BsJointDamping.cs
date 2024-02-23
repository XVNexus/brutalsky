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

        public static BsJointDamping Parse(string raw)
        {
            var parts = raw.Split(' ');
            return Damped(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return $"{ratio} {frequency}";
        }
    }
}
