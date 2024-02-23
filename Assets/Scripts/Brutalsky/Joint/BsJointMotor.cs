namespace Brutalsky.Joint
{
    public struct BsJointMotor
    {
        public bool use { get; set; }
        public float speed { get; set; }
        public float maxForce { get; set; }

        private BsJointMotor(bool use, float speed, float maxForce)
        {
            this.use = use;
            this.speed = speed;
            this.maxForce = maxForce;
        }

        public static BsJointMotor Powered(float speed, float maxForce = 10000f)
        {
            return new BsJointMotor(true, speed, maxForce);
        }

        public static BsJointMotor Unpowered()
        {
            return new BsJointMotor(false, 0f, 0f);
        }

        public static BsJointMotor Parse(string raw)
        {
            if (raw == "x") return Unpowered();
            var parts = raw.Split(' ');
            return Powered(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            return use ? $"{speed} {maxForce}" : "x";
        }
    }
}
