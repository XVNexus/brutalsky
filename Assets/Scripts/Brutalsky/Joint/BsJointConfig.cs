namespace Brutalsky.Joint
{
    public struct BsJointConfig
    {
        public float value { get; set; }
        public bool autoConfig { get; set; }

        public BsJointConfig(float value, bool autoConfig)
        {
            this.value = value;
            this.autoConfig = autoConfig;
        }
    }
}
