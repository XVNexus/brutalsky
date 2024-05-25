using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Config;

public class ConfigList : ILcsDocument
{
    public IdList<ConfigTab> Tabs { get; } = new();

    public ConfigList(params ConfigTab[] tabs)
    {
        foreach (var tab in tabs)
        {
            Tabs.Add(tab);
        }
    }

    public ConfigList() { }

    public ConfigTab this[string id] => Tabs[id];

    public Dictionary<string, object> GetAllChanged()
    {
        var result = new Dictionary<string, object>();
        foreach (var tab in Tabs.Values)
        {
            foreach (var kv in tab.GetAllChanged())
            {
                result[tab.Id + '.' + kv.Key] = kv.Value;
            }
        }
        return result;
    }

    public LcsDocument _ToLcs()
    {
        return new LcsDocument(1, new[] { "#", "$" }, Tabs.Values.Select(LcsLine.Serialize).ToArray());
    }

    public void _FromLcs(LcsDocument document)
    {
        foreach (var tab in document.Lines.Select(LcsLine.Deserialize<ConfigTab>))
        {
            Tabs.Add(tab);
        }
    }
}
