using System.Collections.Generic;

namespace Brutalsky.Scripts.Data;

public abstract class BsObject
{
    public abstract string Tag { get; }
    public string Id { get; set; }
    public List<BsObject> References { get; } = new();
}
