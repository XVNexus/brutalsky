using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;

namespace Brutalsky.Scripts.Config;

public class ConfigSetting : ILcsLine, IHasId
{
    public string Id { get; set; } = "";

    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            Changed = true;
        }
    }

    private object _value = false;
    public bool Changed { get; private set; }

    public ConfigSetting(string id, object value)
    {
        Id = id;
        Value = value;
    }

    public ConfigSetting() { }

    public object? GetIfChanged()
    {
        var result = Changed ? Value : null;
        Changed = false;
        return result;
    }

    public string Stringify()
    {
        return LcsInfo.Stringify(Value);
    }

    public bool Parse(string raw)
    {
        try
        {
            Value = LcsInfo.Parse(raw);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public LcsLine _ToLcs()
    {
        return new LcsLine('$', Id, Value);
    }

    public void _FromLcs(LcsLine line)
    {
        Id = (string)line.Props[0];
        Value = line.Props[1];
    }
}
