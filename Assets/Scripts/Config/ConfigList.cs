using System.Collections.Generic;
using System.Linq;
using Lcs;
using Utils;

namespace Config
{
    public class ConfigList : ILcsDocument
    {
        public IdList<ConfigSection> Sections { get; } = new();

        public ConfigList(List<ConfigSection> sections)
        {
            Sections = new IdList<ConfigSection>();
            foreach (var section in sections)
            {
                Sections.Add(section);
            }
        }

        public ConfigList() { }

        public ConfigSection this[string sectionId] => Sections[sectionId];

        public LcsDocument _ToLcs()
        {
            return new LcsDocument(1, new[] { "#", "$" }, Sections.Values.Select(LcsLine.Serialize).ToArray());
        }

        public void _FromLcs(LcsDocument document)
        {
            foreach (var section in document.Lines.Select(LcsLine.Deserialize<ConfigSection>))
            {
                Sections.Add(section);
            }
        }
    }
}
