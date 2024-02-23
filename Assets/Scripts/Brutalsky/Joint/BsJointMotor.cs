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
    }
}
