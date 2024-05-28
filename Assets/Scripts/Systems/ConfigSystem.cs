using System.Collections.Generic;
using Config;
using Controllers.Base;
using Lcs;

namespace Systems
{
    public class ConfigSystem : BsBehavior
    {
        // Static instance
        public static ConfigSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public ConfigSectionBlueprint[] blueprints;
        public byte saveFormat;

        // Exposed properties
        public ConfigList List { get; private set; }
        public Dictionary<(string, string), string> NameTable { get; } = new();

        // Init functions
        protected override void OnStart()
        {
            // Construct the config tree from the blueprints
            List = new ConfigList(new List<ConfigSection>());
            foreach (var sectionBlueprint in blueprints)
            {
                var section = new ConfigSection(sectionBlueprint.id);
                NameTable[(section.Id, "")] = sectionBlueprint.name;
                foreach (var optionBlueprint in sectionBlueprint.options)
                {
                    var option = new ConfigOption(optionBlueprint.id, LcsInfo.Parse(optionBlueprint.value));
                    NameTable[(section.Id, option.Id)] = optionBlueprint.name;
                    section.Options.Add(option);
                }
                List.Sections.Add(section);
            }
        }

        protected override void OnLoad()
        {
            // Load existing config if available and resave to disk
            if (ResourceSystem._.HasFile("Config", "Options"))
            {
                LoadFile();
            }
            SaveFile();
        }

        // System functions
        public void SaveFile()
        {
            ResourceSystem._.SaveFile("Config", "Options", LcsDocument.Serialize(List), saveFormat);
            EventSystem._.EmitConfigUpdate(new ConfigDelta(List));
        }

        public void LoadFile()
        {
            var loadedList = LcsDocument.Deserialize<ConfigList>(ResourceSystem._.LoadFile("Config", "Options"));
            foreach (var loadedSection in loadedList.Sections.Values)
            {
                if (!List.Sections.Contains(loadedSection)) continue;
                var section = List.Sections[loadedSection.Id];
                foreach (var loadedOption in loadedSection.Options.Values)
                {
                    if (!section.Options.Contains(loadedOption)) continue;
                    var option = section.Options[loadedOption.Id];
                    option.Value = loadedOption.Value;
                }
            }
        }
    }
}
