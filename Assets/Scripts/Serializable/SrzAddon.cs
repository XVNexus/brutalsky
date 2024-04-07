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
            pr = SrzUtils.CompressProperties(properties);
        }

        public SrzAddon()
        {
        }
    }
}
