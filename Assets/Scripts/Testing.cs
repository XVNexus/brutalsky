using Brutalsky;
using Brutalsky.Object;
using Brutalsky.Shape;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        var map = BsMap.Parse(MapSystem.Load("Brutalsky", true));
        MapSystem.current.Build(map);
        PlayerSystem.current.Spawn(map, new[]
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
        var map = new BsMap($"{title} {name}", "Xveon")
        {
            size = new Vector2(size, size / 2f),
            lighting = new BsColor(1f, 1f, 1f, .8f)
        };
        if (bottom)
            map.Add(new BsShape("wall-bottom", new BsTransform(0f, -size / 4f + .5f), BsPath.Rectangle(size, 1f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top)
            map.Add(new BsShape("wall-top", new BsTransform(0f, size / 4f - .5f), BsPath.Rectangle(size, 1f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (left)
            map.Add(new BsShape("wall-left", new BsTransform(-size / 2f + .5f, 0f), BsPath.Rectangle(1f, size / 2f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (right)
            map.Add(new BsShape("wall-right", new BsTransform(size / 2f - .5f, 0f), BsPath.Rectangle(1f, size / 2f),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top && left)
            map.Add(new BsShape("corner-tl", new BsTransform(-size / 2f + 1f, size / 4f - 1f), BsPath.Path("0 0 L 3 0 C 0 0 0 -3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (top && right)
            map.Add(new BsShape("corner-tr", new BsTransform(size / 2f - 1f, size / 4f - 1f), BsPath.Path("0 0 L -3 0 C 0 0 0 -3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (bottom && left)
            map.Add(new BsShape("corner-bl", new BsTransform(-size / 2f + 1f, -size / 4f + 1f), BsPath.Path("0 0 L 3 0 C 0 0 0 3"),
                BsMaterial.Stone(), BsColor.Stone()));
        if (bottom && right)
            map.Add(new BsShape("corner-br", new BsTransform(size / 2f - 1f, -size / 4f + 1f), BsPath.Path("0 0 L -3 0 C 0 0 0 3"),
                BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsSpawn("spawn-left", new BsTransform(-1f, -size / 4f + 1.5f)));
        map.Add(new BsSpawn("spawn-right", new BsTransform(1f, -size / 4f + 1.5f)));
        MapSystem.Save(map.Stringify(), $"{title}{name}", true);
    }
}
