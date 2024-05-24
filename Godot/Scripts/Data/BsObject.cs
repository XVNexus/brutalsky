using System;
using System.Collections.Generic;
using Brutalsky.Scripts.Lcs;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsObject : ILcsLine
{
    public const int TypeGoal = 0;
    public const int TypeDecal = 1;
    public const int TypeShape = 2;
    public const int TypePool = 3;
    public const int TypeSensor = 4;
    public const int TypeMount = 5;
    public const int TypeWeld = 6;
    public const int TypeHinge = 7;
    public const int TypeSlider = 8;
    public const int TypeSpring = 9;

    public int Type { get; set; }
    public string Id { get; set; } = "";
    public string[] References { get; set; } = Array.Empty<string>();
    protected object[] Props { get; set; } = Array.Empty<object>();

    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; }
    public sbyte Layer { get; set; }

    public BsObject(int type, string id, params string[] references)
    {
        Type = type;
        Id = id;
        References = references;
    }

    public BsObject() { }

    public LcsLine _ToLcs()
    {
        var result = new List<object> { Type, Id };
        var referenceArray = new object[References.Length];
        for (var i = 0; i < References.Length; i++)
        {
            referenceArray[i] = References[i];
        }
        result.Add(referenceArray);
        result.Add(new object[] { Position.X, Position.Y });
        result.Add(Rotation);
        result.Add(Layer);
        result.AddRange(Props);
        return new LcsLine('#', result);
    }

    public void _FromLcs(LcsLine line)
    {
        var i = 0;
        Type = (int)line.Props[i++];
        Id = (string)line.Props[i++];
        var rawReferences = (object[])line.Props[i++];
        References = new string[rawReferences.Length];
        for (var j = 0; j < rawReferences.Length; j++)
        {
            References[j] = (string)rawReferences[j];
        }
        var rawPosition = (object[])line.Props[i++];
        Position = new Vector2((float)rawPosition[0], (float)rawPosition[1]);
        Rotation = (float)line.Props[i++];
        Layer = (sbyte)line.Props[i++];
        Props = new object[line.Props.Length - 3];
        while (i < line.Props.Length)
        {
            Props[i - 3] = line.Props[i];
        }
    }
}
