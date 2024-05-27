using System.Collections.Generic;
using Controllers.Base;
using Utils.Config;
using Utils.Lcs;

namespace Systems
{
    public class ConfigSystem : BsBehavior
    {
        // Static instance
        public static ConfigSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public ConfigSectionBlueprint[] blueprints;
        public LcsFormat saveFormat;

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
                    section.AddOption(option);
                }
                List.AddSection(section);
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
            ResourceSystem._.SaveFile("Config", "Options", List.ToLcs(), saveFormat);
            EventSystem._.EmitConfigUpdate(List);
        }

        public void LoadFile()
        {
            var loadedList = ConfigList.FromLcs(ResourceSystem._.LoadFile("Config", "Options"));
            foreach (var loadedSection in loadedList.Sections.Values)
            {
                if (!List.ContainsSection(loadedSection)) continue;
                var section = List.GetSection(loadedSection.Id);
                foreach (var loadedOption in loadedSection.Options.Values)
                {
                    if (!section.ContainsOption(loadedOption)) continue;
                    var option = section.GetOption(loadedOption.Id);
                    option.Value = loadedOption.Value;
                }
            }
        }
    }
}
