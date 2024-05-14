using Utils.Lcs;

namespace Utils.Config
{
    [System.Serializable]
    public class ConfigOptionBlueprint
    {
        public string id;
        public string name;
        public LcsType type;
        public string value;
    }
}
