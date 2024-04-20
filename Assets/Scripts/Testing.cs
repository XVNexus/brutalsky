using Brutalsky;
using Core;
using UnityEngine;
using Utils.Map;
using Utils.Object;
using Utils.Shape;

public class Testing : MonoBehaviour
{
    private void Awake()
    {
        GenerateDefaultMaps();
    }

    private void Start()
    {
        MapSystem._.Build("Brutalsky", "Xveon");
        PlayerSystem._.SpawnAll(MapSystem._.ActiveMap, new[]
        {
            new BsPlayer("Player 1", 100f, new ObjectColor(1f, .5f, 0f)),
            new BsPlayer("Player 2", 100f, new ObjectColor(0f, .5f, 1f), true)
        });
    }

    // TODO: THIS FUNCTION IS DOGSHIT
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

    // TODO: THIS ONE IS TOO
    private void GenerateBoxMap(string title, float size, string name, bool bottom, bool top, bool left, bool right)
    {
        var map = new BsMap($"{name} {title}", "Brutalsky")
        {
            PlayArea = new Vector2(size, size / 2f),
            BackgroundColor = new ObjectColor(.1f, .1f, .1f),
            LightingColor = new ObjectColor(1f, 1f, 1f, .5f),
            GravityDirection = MapGravity.Down,
            GravityStrength = 20f,
            PlayerHealth = 100f
        };
        if (bottom) map.AddObject(new BsShape
        (
            "wall-bottom",
            new ObjectTransform(0f, -size / 4f + .5f),
            ObjectLayer.Midground,
            true,
            Form.Rectangle(size, 1f),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (top) map.AddObject(new BsShape
        (
            "wall-top",
            new ObjectTransform(0f, size / 4f - .5f),
            ObjectLayer.Midground,
            true,
            Form.Rectangle(size, 1f),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (left) map.AddObject(new BsShape
        (
            "wall-left",
            new ObjectTransform(-size / 2f + .5f, 0f),
            ObjectLayer.Midground,
            true,
            Form.Rectangle(1f, size / 2f),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (right) map.AddObject(new BsShape
        (
            "wall-right",
            new ObjectTransform(size / 2f - .5f, 0f),
            ObjectLayer.Midground,
            true,
            Form.Rectangle(1f, size / 2f),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (top && left) map.AddObject(new BsShape
        (
            "corner-tl",
            new ObjectTransform(-size / 2f + 1f, size / 4f - 1f),
            ObjectLayer.Midground,
            true,
            Form.Vector("0 0 0 3 0 1 0 0 0 -3"),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (top && right) map.AddObject(new BsShape
        (
            "corner-tr",
            new ObjectTransform(size / 2f - 1f, size / 4f - 1f),
            ObjectLayer.Midground,
            true,
            Form.Vector("0 0 0 -3 0 1 0 0 0 -3"),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (bottom && left) map.AddObject(new BsShape
        (
            "corner-bl",
            new ObjectTransform(-size / 2f + 1f, -size / 4f + 1f),
            ObjectLayer.Midground,
            true,
            Form.Vector("0 0 0 3 0 1 0 0 0 3"),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        if (bottom && right) map.AddObject(new BsShape
        (
            "corner-br",
            new ObjectTransform(size / 2f - 1f, -size / 4f + 1f),
            ObjectLayer.Midground,
            true,
            Form.Vector("0 0 0 -3 0 1 0 0 0 3"),
            ShapeMaterial.Stone(),
            ObjectColor.Ether()
        ));
        map.AddSpawn(new BsSpawn(new Vector2(-1f, -size / 4f + 1.5f)));
        map.AddSpawn(new BsSpawn(new Vector2(1f, -size / 4f + 1.5f)));
        MapSystem.Save(map.Stringify(), $"{title}{name}");
    }
}
