namespace Utils.Joint
{
    public struct JointMotor
    {
        public bool Use { get; set; }
        public float Speed { get; set; }
        public float MaxForce { get; set; }

        private JointMotor(bool use, float speed, float maxForce)
        {
            Use = use;
            Speed = speed;
            MaxForce = maxForce;
        }

        public static JointMotor Powered(float speed, float maxForce = 10000f) => new(true, speed, maxForce);

        public static JointMotor Unpowered() => new(false, 0f, 0f);
    }
}
