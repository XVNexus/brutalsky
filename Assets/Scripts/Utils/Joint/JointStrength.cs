namespace Utils.Joint
{
    public struct JointStrength
    {
        public float BreakForce { get; set; }
        public float BreakTorque { get; set; }

        private JointStrength(float breakForce, float breakTorque)
        {
            BreakForce = breakForce;
            BreakTorque = breakTorque;
        }

        public static JointStrength Breakable(float breakForce, float breakTorque = float.PositiveInfinity) => new(breakForce, breakTorque);

        public static JointStrength Unbreakable() => new(float.PositiveInfinity, float.PositiveInfinity);
    }
}
