namespace Brutalsky.Joint
{
    public struct BsJointConfig
    {
        public float Value { get; set; }
        public bool Auto { get; set; }

        private BsJointConfig(float value, bool auto)
        {
            Value = value;
            Auto = auto;
        }

        public static BsJointConfig SetValue(float value) => new(value, false);

        public static BsJointConfig AutoValue() => new(0f, true);

        public static BsJointConfig Parse(string raw)
        {
            return raw == "x" ? AutoValue() : SetValue(float.Parse(raw));
        }

        public override string ToString()
        {
            return Auto ? "x" : Value.ToString();
        }
    }
}
