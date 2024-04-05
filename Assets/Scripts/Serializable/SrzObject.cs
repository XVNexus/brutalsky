using System.Collections.Generic;

namespace Serializable
{
    public class SrzObject
    {
        public string id { get; set; }
        public Dictionary<string, string> pr { get; } = new();
        public List<SrzAddon> ad { get; } = new();

        public SrzObject(string tag, string id, Dictionary<string, string> properties, List<SrzAddon> addons)
        {
            this.id = $"{tag}:{id}";
            pr = properties;
            ad = addons;
        }
    }
}
