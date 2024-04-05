namespace Utils.Joint
{
    public struct JointConfig
    {
        public float Value { get; set; }
        public bool Auto { get; set; }

        private JointConfig(float value, bool auto)
        {
            Value = value;
            Auto = auto;
        }

        public static JointConfig SetValue(float value) => new(value, false);

        public static JointConfig AutoValue() => new(0f, true);

        public static JointConfig Parse(string raw)
        {
            return raw == "x" ? AutoValue() : SetValue(float.Parse(raw));
        }

        public override string ToString()
        {
            return Auto ? "x" : Value.ToString();
        }
    }
}
