using Brutalsky;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public void Start()
    {
        var map = new BsMap();

        // Decorative shapes
        map.shapes.Add(new BsShape(new BsTransform(-2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(new Color(.3f, .3f, .3f), BsLayer.Background), false));
        map.shapes.Add(new BsShape(new BsTransform(2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(new Color(.3f, .3f, .3f), BsLayer.Background), false));

        // Structure shapes
        map.shapes.Add(new BsShape(new BsTransform(-20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.FromString("15 1 L 15 .25 L 14.25 -.5 L -14.25 -.5 L -15 .25 L -15 1"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-19f, 0f), BsPath.FromString("-1 2.5 L 0 2.5 C 0 .5 2 .5 L 8 .5 L 8.5 0 L 8 -.5 L 2 -.5 C 0 -.5 0 -2.5 L -1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(19f, 0f), BsPath.FromString("1 2.5 L 0 2.5 C 0 .5 -2 .5 L -8 .5 L -8.5 0 L -8 -.5 L -2 -.5 C 0 -.5 0 -2.5 L 1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-13f, -10f), BsPath.FromString("-6.5 -2 L -6.5 -1 L -6 -1 L -6 -1.5 L 6 -1.5 L 6 -1 L 6.5 -1 L 6.5 -2"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(13f, -10f), BsPath.FromString("-6.5 -2 L -6.5 -1 L -6 -1 L -6 -1.5 L 6 -1.5 L 6 -1 L 6.5 -1 L 6.5 -2"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-17f, 10f), BsPath.FromString("-2.5 2 L -2.5 1 L -2 1 L -2 1.5 L 2 1.5 L 2 1 L 2.5 1 L 2.5 2"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(17f, 10f), BsPath.FromString("-2.5 2 L -2.5 1 L -2 1 L -2 1.5 L 2 1.5 L 2 1 L 2.5 1 L 2.5 2"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(0f, -10f), BsPath.FromString("-7 -1 L -7 .5 L 0 3 L 7 .5 L 7 -1"),
            BsMaterial.Stone(), BsColor.Stone()));

        // Interactive shapes
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.FromString("-14.25 -.5 L -15 .25 L -15 -.5 L -14.5 -1 L -1 -1 L -1 -.5"),
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.FromString("14.25 -.5 L 15 .25 L 15 -.5 L 14.5 -1 L 1 -1 L 1 -.5"),
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(0, -9.5f), BsPath.FromString("-7 0 L -7 .5 L 0 3 L 7 .5 L 7 0 L 0 2.5"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(0, 9.25f), BsPath.FromString("1 .25 L 1 -.25 L 0 -1.25 L -1 -.25 L -1 .25"),
            BsMaterial.Rubber(), BsColor.Rubber()));

        // Dynamic shapes
        map.shapes.Add(new BsShape(new BsTransform(-2f, 0f, 0f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal()));
        map.shapes.Add(new BsShape(new BsTransform(2f, 0f, 30f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal()));

        // Pools
        map.pools.Add(new BsPool(new BsTransform(-13f, -10.25f), new Vector2(12f, 2.5f),
            BsChemical.Water(), BsColor.Water()));
        map.pools.Add(new BsPool(new BsTransform(13f, -10.25f), new Vector2(12f, 2.5f),
            BsChemical.Water(), BsColor.Water()));
        map.pools.Add(new BsPool(new BsTransform(-17f, 10.5f, 180f), new Vector2(4f, 2f),
            BsChemical.Honey(), BsColor.Honey()));
        map.pools.Add(new BsPool(new BsTransform(17f, 10.5f, 180f), new Vector2(4f, 2f),
            BsChemical.Honey(), BsColor.Honey()));

        MapSystem.current.Load(map);
    }
}
