using System.Collections.Generic;
using Utils;

namespace Serializable
{
    public class SrzAddon
    {
        public string id { get; set; }
        public Dictionary<string, string> pr { get; set; } = new();

        public SrzAddon(string tag, string id, Dictionary<string, string> properties)
        {
            this.id = BsUtils.GenerateFullId(tag, id);
            pr = properties;
        }

        public SrzAddon()
        {
        }
    }
}
