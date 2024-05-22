using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Config;

public class ConfigDocument : ILcsDocument
{
    public IdList<string, ConfigTab> Tabs { get; } = new();

    public ConfigDocument(List<ConfigTab> tabs)
    {
        Tabs = new IdList<string, ConfigTab>();
        foreach (var tab in tabs)
        {
            Tabs[tab.Id] = tab;
        }
    }

    public ConfigDocument() { }

    public ConfigTab this[string tabId] => Tabs[tabId];

    public LcsDocument _ToLcs()
    {
        return new LcsDocument(1, Tabs.Values.Select(LcsInfo.Serialize).ToList(), new[] { "#", "$" });
    }

    public void _FromLcs(LcsDocument document)
    {
        foreach (var tab in document.Lines.Select(LcsInfo.Parse<ConfigTab>))
        {
            Tabs.Add(tab.Id, tab);
        }
    }
}
