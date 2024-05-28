namespace Config
{
    [System.Serializable]
    public class ConfigSectionBlueprint
    {
        public string id;
        public string name;
        public ConfigOptionBlueprint[] options;
    }
}
