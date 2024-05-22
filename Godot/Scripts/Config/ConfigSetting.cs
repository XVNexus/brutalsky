using Brutalsky.Scripts.Lcs;

namespace Brutalsky.Scripts.Config;

public class ConfigSetting : ILcsLine
{
    public string Id { get; private set; } = "";
    public object? Value { get; set; }

    public ConfigSetting(string id, object? value)
    {
        Id = id;
        Value = value;
    }

    public ConfigSetting() { }

    public string Stringify()
    {
        return LcsInfo.Stringify(Value ?? "");
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
        return new LcsLine('$', Id, Value ?? "");
    }

    public void _FromLcs(LcsLine line)
    {
        Id = (string)line.Props[0];
        Value = line.Props[1];
    }
}
