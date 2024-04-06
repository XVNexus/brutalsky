using System.Collections.Generic;
using Utils;

namespace Serializable
{
    public class SrzAddon
    {
        public string id { get; set; }
        public string pr { get; set; }

        public SrzAddon(string tag, string id, string[] properties)
        {
            this.id = BsUtils.GenerateFullId(tag, id);
            pr = BsUtils.CompressProperties(properties);
        }

        public SrzAddon()
        {
        }
    }
}
