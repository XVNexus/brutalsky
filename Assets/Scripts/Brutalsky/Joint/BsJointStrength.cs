namespace Brutalsky.Joint
{
    public struct BsJointStrength
    {
        public float BreakForce { get; set; }
        public float BreakTorque { get; set; }

        private BsJointStrength(float breakForce, float breakTorque)
        {
            BreakForce = breakForce;
            BreakTorque = breakTorque;
        }

        public static BsJointStrength Breakable(float breakForce, float breakTorque = float.PositiveInfinity) => new(breakForce, breakTorque);

        public static BsJointStrength Unbreakable() => new(float.PositiveInfinity, float.PositiveInfinity);

        public static BsJointStrength Parse(string raw)
        {
            if (raw == "x") return Unbreakable();
            var parts = raw.Split(' ');
            return Breakable(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            var breakable = !float.IsPositiveInfinity(BreakForce) || !float.IsPositiveInfinity(BreakTorque);
            return breakable ? $"{BreakForce} {BreakTorque}" : "x";
        }
    }
}
