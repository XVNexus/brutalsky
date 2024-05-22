using System;
using System.Collections.Generic;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Map;

public class BsMap
{
    public uint Id => GenerateId(Title, Author);
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public Rect2 PlayArea { get; set; } = new(-10f, -10f, 20f, 20f);
    public Color BackgroundColor { get; set; } = new(.25f, .25f, .25f);
    public Color LightingColor = new(1f, 1f, 1f, .8f);
    public Color LightingTint
    {
        get => LightingColor.StripAlpha();
        set => LightingColor = new Color(value.R, value.G, value.B, LightingColor.A);
    }
    public float LightingIntensity
    {
        get => LightingColor.A;
        set => LightingColor = new Color(LightingColor.R, LightingColor.G, LightingColor.B, value);
    }
    public Direction GravityDirection { get; set; } = Direction.None;
    public float GravityStrength { get; set; } = 0f;
    public float AirResistance { get; set; } = 0f;
    public float PlayerHealth { get; set; } = 100f;
    public bool AllowDummies { get; set; } = true;

    public List<BsSpawn> Spawns { get; } = new();
    public Dictionary<(string, string), BsObject> Objects { get; } = new();
    public List<BsNode> Nodes { get; } = new();
    public Dictionary<BsPort, BsLink> Links { get; } = new();

    // Util functions
    public static Vector2 GravityToVector(Direction direction, float strength)
    {
        return direction switch
        {
            Direction.Down => Vector2.Down,
            Direction.Up => Vector2.Up,
            Direction.Left => Vector2.Left,
            Direction.Right => Vector2.Right,
            _ => Vector2.Zero
        } * strength;
    }

    public static uint GenerateId(string title, string author)
    {
        return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
    }
}
