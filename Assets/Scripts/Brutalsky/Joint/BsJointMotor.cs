namespace Brutalsky.Joint
{
    public struct BsJointMotor
    {
        public bool Use { get; set; }
        public float Speed { get; set; }
        public float MaxForce { get; set; }

        private BsJointMotor(bool use, float speed, float maxForce)
        {
            Use = use;
            Speed = speed;
            MaxForce = maxForce;
        }

        public static BsJointMotor Powered(float speed, float maxForce = 10000f) => new(true, speed, maxForce);

        public static BsJointMotor Unpowered() => new(false, 0f, 0f);

        public static BsJointMotor Parse(string raw)
        {
            if (raw == "x") return Unpowered();
            var parts = raw.Split(' ');
            return Powered(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return Use ? $"{Speed} {MaxForce}" : "x";
        }
    }
}
