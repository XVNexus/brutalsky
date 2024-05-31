using System.Collections.Generic;
using System.Linq;
using Lcs;
using Utils;

namespace Config
{
    public class ConfigSection : IHasId, ILcsLine
    {
        public string Id { get; set; } = "";
        public IdList<ConfigOption> Options { get; } = new();

        public ConfigSection(string id, List<ConfigOption> options)
        {
            Id = id;
            Options = new IdList<ConfigOption>();
            foreach (var option in options)
            {
                Options.Add(option);
            }
        }

        public ConfigSection(string id)
        {
            Id = id;
            Options = new IdList<ConfigOption>();
        }

        public ConfigSection() { }

        public ConfigOption this[string optionId] => Options[optionId];

        public LcsLine _ToLcs()
        {
            return new LcsLine('#', new object[] { Id }, Options.Values.Select(LcsLine.Serialize).ToList());
        }

        public void _FromLcs(LcsLine line)
        {
            Id = (string)line.Props[0];
            foreach (var option in line.Children.Select(LcsLine.Deserialize<ConfigOption>))
            {
                Options.Add(option);
            }
        }
    }
}
