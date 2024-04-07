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
    }
}
