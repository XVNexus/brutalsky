using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Config;

public class ConfigTab : ILcsLine, IHasId
{
    public string Id { get; set; } = "";
    public IdList<ConfigSetting> Settings { get; } = new();

    public ConfigTab(string id, params ConfigSetting[] settings)
    {
        Id = id;
        foreach (var setting in settings)
        {
            Settings.Add(setting);
        }
    }

    public ConfigTab() { }

    public ConfigSetting this[string id] => Settings[id];

    public Dictionary<string, object> GetAllChanged()
    {
        var result = new Dictionary<string, object>();
        foreach (var setting in Settings.Values)
        {
            var valueIfChanged = setting.GetIfChanged();
            if (valueIfChanged != null)
            {
                result[setting.Id] = valueIfChanged;
            }
        }
        return result;
    }

    public LcsLine _ToLcs()
    {
        return new LcsLine('#', new object[] { Id }, Settings.Values.Select(LcsLine.Serialize));
    }

    public void _FromLcs(LcsLine line)
    {
        Id = (string)line.Props[0];
        foreach (var setting in line.Children.Select(LcsLine.Deserialize<ConfigSetting>))
        {
            Settings.Add(setting);
        }
    }
}
