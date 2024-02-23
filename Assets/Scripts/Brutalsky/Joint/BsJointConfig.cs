namespace Brutalsky.Joint
{
    public struct BsJointConfig
    {
        public float value { get; set; }
        public bool autoConfig { get; set; }

        private BsJointConfig(float value, bool autoConfig)
        {
            this.value = value;
            this.autoConfig = autoConfig;
        }

        public static BsJointConfig Value(float value)
        {
            return new BsJointConfig(value, false);
        }

        public static BsJointConfig Auto()
        {
            return new BsJointConfig(0f, true);
        }
    }
}
