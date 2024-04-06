using System.Collections.Generic;

namespace Serializable
{
    public class SrzMap
    {
        public string tt { get; set; }
        public string at { get; set; }
        public string sz { get; set; }
        public string lg { get; set; }
        public List<SrzSpawn> sp { get; set; } = new();
        public List<SrzObject> ob { get; set; } = new();

        public SrzMap(string title, string author, string size, string lighting, List<SrzSpawn> spawns, List<SrzObject> objects)
        {
            tt = title;
            at = author;
            sz = size;
            lg = lighting;
            sp = spawns;
            ob = objects;
        }

        public SrzMap()
        {
        }
    }
}
