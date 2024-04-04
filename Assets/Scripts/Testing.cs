using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        GenerateDefaultMaps();
        var map = BsMap.Parse(MapSystem.LoadInternal("Brutalsky"));
        MapSystem._.Build(map);
        PlayerSystem._.Spawn(map, new[]
        {
            new BsPlayer("player-1", "Player 1", 100f, new BsColor(1f, .5f, 0f)),
            new BsPlayer("player-2", "Player 2", 100f, new BsColor(0f, .5f, 1f), true)
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
            Lighting = new BsColor(1f, 1f, 1f, .8f)
        };
        if (bottom)
            map.AddObject(new BsShape("wall-bottom", new BsTransform(0f, -size / 4f + .5f), BsPath.Rectangle(size, 1f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top)
            map.AddObject(new BsShape("wall-top", new BsTransform(0f, size / 4f - .5f), BsPath.Rectangle(size, 1f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (left)
            map.AddObject(new BsShape("wall-left", new BsTransform(-size / 2f + .5f, 0f), BsPath.Rectangle(1f, size / 2f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (right)
            map.AddObject(new BsShape("wall-right", new BsTransform(size / 2f - .5f, 0f), BsPath.Rectangle(1f, size / 2f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top && left)
            map.AddObject(new BsShape("corner-tl", new BsTransform(-size / 2f + 1f, size / 4f - 1f), BsPath.Path("0 0 L 3 0 C 0 0 0 -3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top && right)
            map.AddObject(new BsShape("corner-tr", new BsTransform(size / 2f - 1f, size / 4f - 1f), BsPath.Path("0 0 L -3 0 C 0 0 0 -3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (bottom && left)
            map.AddObject(new BsShape("corner-bl", new BsTransform(-size / 2f + 1f, -size / 4f + 1f), BsPath.Path("0 0 L 3 0 C 0 0 0 3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (bottom && right)
            map.AddObject(new BsShape("corner-br", new BsTransform(size / 2f - 1f, -size / 4f + 1f), BsPath.Path("0 0 L -3 0 C 0 0 0 3"),
                BsMaterial.Stone(), BsColor.Stone()));
        map.AddSpawn(new BsSpawn(new Vector2(-1f, -size / 4f + 1.5f)));
        map.AddSpawn(new BsSpawn(new Vector2(1f, -size / 4f + 1.5f)));
        MapSystem.Save(map.Stringify(), $"{title}{name}");
    }
}
