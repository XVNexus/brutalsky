using System.Collections.Generic;
using Godot;

namespace Brutalsky.Scripts.Map;

public abstract class BsObject
{
    public abstract PackedScene Prefab { get; }
    public abstract string Tag { get; }
    public string Id { get; set; }
    public (string, string)? Parent { get; }
    public List<(string, string)> Siblings { get; } = new();

    public Node? InstanceNode { get; private set; } = null;
    public bool Active { get; private set; } = false;

    protected BsObject(string id, (string, string) parent, params (string, string)[] siblings)
    {
        Id = id;
        Parent = parent;
        Siblings.AddRange(siblings);
    }

    protected BsObject(string id, (string, string) parent)
    {
        Id = id;
        Parent = parent;
    }

    protected BsObject(string id)
    {
        Id = id;
    }

    protected abstract void __Init();

    protected abstract object[] _ToLcs();

    protected abstract void _FromLcs(object[] props);

    public void Activate()
    {
        if (Parent != null)
        {
            
        }
    }
}
