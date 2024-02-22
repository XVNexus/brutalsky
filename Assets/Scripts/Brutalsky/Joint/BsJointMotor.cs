namespace Brutalsky.Joint
{
    public struct BsJointMotor
    {
        public bool use { get; set; }
        public float speed { get; set; }
        public float maxForce { get; set; }

        public BsJointMotor(bool use, float speed, float maxForce)
        {
            this.use = use;
            this.speed = speed;
            this.maxForce = maxForce;
        }
    }
}
