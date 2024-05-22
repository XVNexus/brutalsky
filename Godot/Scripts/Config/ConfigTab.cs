using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Config;

public class ConfigTab : ILcsLine
{
    public string Id { get; private set; } = "";
    public IdList<string, ConfigSetting> Settings { get; } = new();

    public ConfigTab(string id, List<ConfigSetting> settings)
    {
        Id = id;
        Settings = new IdList<string, ConfigSetting>();
        foreach (var setting in settings)
        {
            Settings.Add(setting.Id, setting);
        }
    }

    public ConfigTab(string id)
    {
        Id = id;
        Settings = new IdList<string, ConfigSetting>();
    }

    public ConfigTab() { }

    public ConfigSetting this[string settingId] => Settings[settingId];

    public LcsLine _ToLcs()
    {
        return new LcsLine('#', new object[] { Id }, Settings.Values.Select(LcsInfo.Serialize).ToList());
    }

    public void _FromLcs(LcsLine line)
    {
        Id = (string)line.Props[0];
        foreach (var setting in line.Children.Select(LcsInfo.Parse<ConfigSetting>))
        {
            Settings.Add(setting.Id, setting);
        }
    }
}
