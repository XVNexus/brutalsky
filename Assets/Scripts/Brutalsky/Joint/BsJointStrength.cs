namespace Brutalsky.Joint
{
    public struct BsJointStrength
    {
        public float breakForce { get; set; }
        public float breakTorque { get; set; }

        private BsJointStrength(float breakForce, float breakTorque)
        {
            this.breakForce = breakForce;
            this.breakTorque = breakTorque;
        }

        public static BsJointStrength Breakable(float breakForce, float breakTorque = float.PositiveInfinity)
        {
            return new BsJointStrength(breakForce, breakTorque);
        }

        public static BsJointStrength Unbreakable()
        {
            return new BsJointStrength(float.PositiveInfinity, float.PositiveInfinity);
        }

        public static BsJointStrength Parse(string raw)
        {
            if (raw == "x") return Unbreakable();
            var parts = raw.Split(' ');
            return Breakable(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public override string ToString()
        {
            var breakable = !float.IsPositiveInfinity(breakForce) || !float.IsPositiveInfinity(breakTorque);
            return breakable ? $"{breakForce} {breakTorque}" : "x";
        }
    }
}
