using System.Collections.Generic;
using System.Linq;
using Lcs;
using Utils.Constants;

namespace Utils.Config
{
    public class ConfigList
    {
        public Dictionary<string, ConfigSection> Sections { get; }

        public ConfigList(List<ConfigSection> sections)
        {
            Sections = new Dictionary<string, ConfigSection>();
            foreach (var section in sections)
            {
                Sections[section.Id] = section;
            }
        }

        public ConfigSection this[string sectionId]
        {
            get => GetSection(sectionId);
        }

        public ConfigSection GetSection(string id)
        {
            return ContainsSection(id) ? Sections[id] : throw Errors.NoItemFound("config section", id);
        }

        public bool AddSection(ConfigSection section)
        {
            if (ContainsSection(section)) return false;
            Sections[section.Id] = section;
            return true;
        }

        public bool RemoveSection(ConfigSection section)
        {
            return RemoveSection(section.Id);
        }

        public bool RemoveSection(string id)
        {
            if (!ContainsSection(id)) return false;
            Sections.Remove(id);
            return true;
        }

        public bool ContainsSection(ConfigSection section)
        {
            return ContainsSection(section.Id);
        }

        public bool ContainsSection(string id)
        {
            return Sections.ContainsKey(id);
        }

        public LcsDocument ToLcs()
        {
            return new LcsDocument(1, new[] { "#", "$" }, Sections.Values.Select(section => section.ToLcs()).ToArray());
        }

        public static ConfigList FromLcs(LcsDocument document)
        {
            return new ConfigList(document.Lines.Select(ConfigSection.FromLcs).ToList());
        }
    }
}
