using Brutalsky;
using Core;
using UnityEngine;
using Utils.Object;
using Utils.Path;
using Utils.Shape;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        GenerateDefaultMaps();
        var map = BsMap.Parse(MapSystem.LoadInternal("Brutalsky"));
        MapSystem._.Build(map);
        PlayerSystem._.Spawn(map, new[]
        {
            new BsPlayer("player-1", "Player 1", 100f, new ObjectColor(1f, .5f, 0f)),
            new BsPlayer("player-2", "Player 2", 100f, new ObjectColor(0f, .5f, 1f), true)
        });
    }

    private void GenerateDefaultMaps()
    {
        var sizes = new[] { 20f, 40f, 80f };
        var names = new[] { "Small", "Medium", "Large" };
        for (var i = 0; i < sizes.Length; i++)
        {
            GenerateBoxMap("Platform", sizes[i], names[i], true, false, false, false);
            GenerateBoxMap("Box", sizes[i], names[i], true, false, true, true);
            GenerateBoxMap("Cage", sizes[i], names[i], true, true, true, true);
            GenerateBoxMap("Tunnel", sizes[i], names[i], true, true, false, false);
        }
    }

    private void GenerateBoxMap(string title, float size, string name, bool bottom, bool top, bool left, bool right)
    {
        var map = new BsMap($"{name} {title}", "Brutalsky")
        {
            Size = new Vector2(size, size / 2f),
            Lighting = new ObjectColor(1f, 1f, 1f, .8f)
        };
        if (bottom)
            map.AddObject(new BsShape("wall-bottom", new ObjectTransform(0f, -size / 4f + .5f), Path.Rectangle(size, 1f),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (top)
            map.AddObject(new BsShape("wall-top", new ObjectTransform(0f, size / 4f - .5f), Path.Rectangle(size, 1f),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (left)
            map.AddObject(new BsShape("wall-left", new ObjectTransform(-size / 2f + .5f, 0f), Path.Rectangle(1f, size / 2f),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (right)
            map.AddObject(new BsShape("wall-right", new ObjectTransform(size / 2f - .5f, 0f), Path.Rectangle(1f, size / 2f),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (top && left)
            map.AddObject(new BsShape("corner-tl", new ObjectTransform(-size / 2f + 1f, size / 4f - 1f), Path.Vector("0 0 L 3 0 C 0 0 0 -3"),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (top && right)
            map.AddObject(new BsShape("corner-tr", new ObjectTransform(size / 2f - 1f, size / 4f - 1f), Path.Vector("0 0 L -3 0 C 0 0 0 -3"),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (bottom && left)
            map.AddObject(new BsShape("corner-bl", new ObjectTransform(-size / 2f + 1f, -size / 4f + 1f), Path.Vector("0 0 L 3 0 C 0 0 0 3"),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        if (bottom && right)
            map.AddObject(new BsShape("corner-br", new ObjectTransform(size / 2f - 1f, -size / 4f + 1f), Path.Vector("0 0 L -3 0 C 0 0 0 3"),
                ShapeMaterial.Stone(), ObjectColor.Stone()));
        map.AddSpawn(new BsSpawn(new Vector2(-1f, -size / 4f + 1.5f)));
        map.AddSpawn(new BsSpawn(new Vector2(1f, -size / 4f + 1.5f)));
        MapSystem.Save(map.Stringify(), $"{title}{name}");
    }
}
