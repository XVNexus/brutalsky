using System.Collections.Generic;

namespace Serializable
{
    public class SrzAddon
    {
        public string id { get; set; }
        public Dictionary<string, string> pr { get; } = new();

        public SrzAddon(string tag, string id, Dictionary<string, string> properties)
        {
            this.id = $"{tag}:{id}";
            pr = properties;
        }
    }
}
