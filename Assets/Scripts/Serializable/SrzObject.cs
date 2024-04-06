using System.Collections.Generic;
using Utils;

namespace Serializable
{
    public class SrzObject
    {
        public string id { get; set; }
        public Dictionary<string, string> pr { get; set; } = new();
        public List<SrzAddon> ad { get; set; } = new();

        public SrzObject(string tag, string id, Dictionary<string, string> properties, List<SrzAddon> addons)
        {
            this.id = BsUtils.GenerateFullId(tag, id);
            pr = properties;
            ad = addons;
        }

        public SrzObject()
        {
        }
    }
}
