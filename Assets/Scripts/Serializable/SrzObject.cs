using System.Collections.Generic;
using Utils;

namespace Serializable
{
    public class SrzObject
    {
        public string id { get; set; }
        public string pr { get; set; }
        public List<SrzAddon> ad { get; set; } = new();

        public SrzObject(string tag, string id, string[] properties, List<SrzAddon> addons)
        {
            this.id = BsUtils.GenerateFullId(tag, id);
            pr = SrzUtils.CompressProperties(properties);
            ad = addons;
        }

        public SrzObject()
        {
        }
    }
}
