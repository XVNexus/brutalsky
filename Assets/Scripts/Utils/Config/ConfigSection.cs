using System.Collections.Generic;
using System.Linq;
using Utils.Constants;
using Utils.Lcs;

namespace Utils.Config
{
    public class ConfigSection
    {
        public string Id { get; }
        public Dictionary<string, ConfigOption> Options { get; }

        public ConfigSection(string id, List<ConfigOption> options)
        {
            Id = id;
            Options = new Dictionary<string, ConfigOption>();
            foreach (var option in options)
            {
                Options[option.Id] = option;
            }
        }

        public ConfigSection(string id)
        {
            Id = id;
            Options = new Dictionary<string, ConfigOption>();
        }

        public ConfigOption this[string optionId]
        {
            get => GetOption(optionId);
        }

        public ConfigOption GetOption(string id)
        {
            return ContainsOption(id) ? Options[id] : throw Errors.NoItemFound("config option", id);
        }

        public bool AddOption(ConfigOption option)
        {
            if (ContainsOption(option)) return false;
            Options[option.Id] = option;
            return true;
        }

        public bool RemoveOption(ConfigOption option)
        {
            return RemoveOption(option.Id);
        }

        public bool RemoveOption(string id)
        {
            if (!ContainsOption(id)) return false;
            Options.Remove(id);
            return true;
        }

        public bool ContainsOption(ConfigOption option)
        {
            return ContainsOption(option.Id);
        }

        public bool ContainsOption(string id)
        {
            return Options.ContainsKey(id);
        }

        public LcsLine ToLcs()
        {
            return new LcsLine('#', new object[] { Id }, Options.Values.Select(option => option.ToLcs()).ToList());
        }

        public static ConfigSection FromLcs(LcsLine line)
        {
            return new ConfigSection((string)line.Props[0],
                line.Children.Select(child => ConfigOption.FromLcs(child)).ToList());
        }
    }
}
