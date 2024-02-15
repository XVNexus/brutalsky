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
        map.shapes.Add(new BsShape(new BsTransform(0f, -10f), BsPath.FromString("-7 -1 L -7 .5 L 0 3 L 7 .5 L 7 -1"),
            BsMaterial.Stone(), BsColor.Stone()));

        // Exterior shapes
        map.shapes.Add(new BsShape(new BsTransform(-14.5f, 12f), BsPath.Rectangle(1f, 2f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(14.5f, 12f), BsPath.Rectangle(1f, 2f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-19f, 13.5f), BsPath.Rectangle(10f, 1f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(19f, 13.5f), BsPath.Rectangle(10f, 1f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-23.5f, 0f), BsPath.Rectangle(1f, 26f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(23.5f, 0f), BsPath.Rectangle(1f, 26f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-15f, -13.5f), BsPath.Rectangle(18f, 1f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(15f, -13.5f), BsPath.Rectangle(18f, 1f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-6.5f, -12f), BsPath.Rectangle(1f, 2f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(6.5f, -12f), BsPath.Rectangle(1f, 2f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-22.5f, 12.25f), BsPath.Rectangle(1f, 1.5f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(22.5f, 12.25f), BsPath.Rectangle(1f, 1.5f),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(-23f, -13f), BsPath.FromString("0 0 L 0 4 C 0 0 2 0"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(23f, -13f), BsPath.FromString("0 0 L 0 4 C 0 0 -2 0"),
            BsMaterial.Stone(), BsColor.Stone()));

        // Interactive shapes
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.FromString("-14.25 -.5 L -15 .25 L -15 -.5 L -14.5 -1 L -4 -1 L -4 -.5"),
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.FromString("14.25 -.5 L 15 .25 L 15 -.5 L 14.5 -1 L 4 -1 L 4 -.5"),
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(0, -9.5f), BsPath.FromString("-7 0 L -7 .5 L 0 3 L 7 .5 L 7 0 L 0 2.5"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(0, 9.25f), BsPath.FromString("4 .25 L 4 -.25 L 0 -1.25 L -4 -.25 L -4 .25"),
            BsMaterial.Rubber(), BsColor.Rubber()));

        // Dynamic shapes
        var spinnerLeft = new BsShape(new BsTransform(-2f, 0f, 0f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal());
        map.shapes.Add(spinnerLeft);
        var spinnerRight = new BsShape(new BsTransform(2f, 0f, 30f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal());
        map.shapes.Add(spinnerRight);

        // Pools
        map.pools.Add(new BsPool(new BsTransform(-17f, 11f, 0f), new Vector2(4f, 3f),
            BsChemical.Honey(), BsColor.Honey(BsLayer.Foreground)));
        map.pools.Add(new BsPool(new BsTransform(17f, 11f, 0f), new Vector2(4f, 3f),
            BsChemical.Honey(), BsColor.Honey(BsLayer.Foreground)));
        map.pools.Add(new BsPool(new BsTransform(-18f, 12f, 90f), new Vector2(2f, 6f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(18f, 12f, 270f), new Vector2(2f, 6f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(-22f, 1f, 180f), new Vector2(2f, 24f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(22f, 1f, 180f), new Vector2(2f, 24f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(-21f, -12f, 270f), new Vector2(2f, 4f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(21f, -12f, 90f), new Vector2(2f, 4f),
            BsChemical.Water(), BsColor.Water(BsLayer.Background)));
        map.pools.Add(new BsPool(new BsTransform(-13f, -11f), new Vector2(12f, 4f),
            BsChemical.Water(), BsColor.Water(BsLayer.Foreground)));
        map.pools.Add(new BsPool(new BsTransform(13f, -11f), new Vector2(12f, 4f),
            BsChemical.Water(), BsColor.Water(BsLayer.Foreground)));

        // Joints
        map.joints.Add(new BsJoint(spinnerLeft, null, new BsTransform(),
            BsJointType.Hinge, 100f, 1000000f));
        map.joints.Add(new BsJoint(spinnerRight, null, new BsTransform(),
            BsJointType.Hinge, -100f, 1000000f));

        MapSystem.current.Load(map);
    }
}
