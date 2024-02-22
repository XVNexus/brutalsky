using System;
using System.IO;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Brutalsky.Pool;
using Brutalsky.Shape;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public void Start()
    {
        var map = new BsMap("Brutalsky", "xveon");

        // Decorative shapes
        map.shapes.Add(new BsShape(new BsTransform(-2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(.3f, .3f, .3f), BsLayer.Background, false));
        map.shapes.Add(new BsShape(new BsTransform(2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(.3f, .3f, .3f), BsLayer.Background, false));

        // Structure shapes
        map.shapes.Add(new BsShape(new BsTransform(-20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(0f, 10f), BsPath.Path("-15 1 L 15 1 L 15 0 L 14 -.5 L -14 -.5 L -15 0"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-19f, 0f), BsPath.Path("-1 2.5 L 0 2.5 C 0 .5 2 .5 L 8 .5 L 7.5 0 L 8 -.5 L 2 -.5 C 0 -.5 0 -2.5 L -1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(19f, 0f), BsPath.Path("1 2.5 L 0 2.5 C 0 .5 -2 .5 L -8 .5 L -7.5 0 L -8 -.5 L -2 -.5 C 0 -.5 0 -2.5 L 1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(0f, -10f), BsPath.Path("-7 -1 L -7 .5 L 0 3 L 7 .5 L 7 -1"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-17f, 13.25f), BsPath.Rectangle(5f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-19.25f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-14.75f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(17f, 13.25f), BsPath.Rectangle(5f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(14.75f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(19.25f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-13f, -13.25f), BsPath.Rectangle(13f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-19.25f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(-6.75f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(13f, -13.25f), BsPath.Rectangle(13f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(6.75f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.shapes.Add(new BsShape(new BsTransform(19.25f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));

        // Interactive shapes
        map.shapes.Add(new BsShape(new BsTransform(-9f, 9.25f), BsPath.Rectangle(10f, .5f), 
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(9f, 9.25f), BsPath.Rectangle(10f, .5f),
            BsMaterial.Glue(), BsColor.Glue()));
        map.shapes.Add(new BsShape(new BsTransform(-14.5f, 9.5f), BsPath.Path("-.5 .5 L .5 0 L .5 -.5 L -.5 0"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(14.5f, 9.5f), BsPath.Path(".5 .5 L -.5 0 L -.5 -.5 L .5 0"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(0f, -9.5f), BsPath.Path("-7 0 L -7 .5 L 0 3 L 7 .5 L 7 0 L 0 2.5"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.shapes.Add(new BsShape(new BsTransform(0f, 9.25f), BsPath.Path("4 .25 L 4 -.25 L 0 -1.25 L -4 -.25 L -4 .25"),
            BsMaterial.Rubber(), BsColor.Rubber()));
        map.shapes.Add(new BsShape(new BsTransform(-11f, 0f, 45f), BsPath.Square(.7f),
            BsMaterial.Electric(), BsColor.Electric()));
        map.shapes.Add(new BsShape(new BsTransform(11f, 0f, 45f), BsPath.Square(.7f),
            BsMaterial.Electric(), BsColor.Electric()));

        // Dynamic shapes
        var spinnerLeft = new BsShape(new BsTransform(-2f, 0f, -15f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal());
        map.shapes.Add(spinnerLeft);
        var spinnerRight = new BsShape(new BsTransform(2f, 0f, 15f), BsPath.Star(6, 5f, 2.8f),
            BsMaterial.Metal(true), BsColor.Metal());
        map.shapes.Add(spinnerRight);
        // map.shapes.Add(new BsShape(new BsTransform(-12f, 5f), BsPath.Rectangle(.5f, 2f),
        //     BsMaterial.Glue(true).Modify(adhesion: 3f), BsColor.Ether()));
        // map.shapes.Add(new BsShape(new BsTransform(12f, 5f), BsPath.Rectangle(.5f, 2f),
        //     BsMaterial.Glue(true).Modify(adhesion: 3f), BsColor.Ether()));

        // Pools
        map.pools.Add(new BsPool(new BsTransform(-17f, 11.25f, 180f), new Vector2(4f, 3.5f),
            BsChemical.Water(), BsColor.Water(), BsLayer.Foreground));
        map.pools.Add(new BsPool(new BsTransform(17f, 11.25f, 180f), new Vector2(4f, 3.5f),
            BsChemical.Water(), BsColor.Water(), BsLayer.Foreground));
        map.pools.Add(new BsPool(new BsTransform(-13f, -11f), new Vector2(12f, 4f),
            BsChemical.Lava(), BsColor.Lava(), BsLayer.Foreground));
        map.pools.Add(new BsPool(new BsTransform(13f, -11f), new Vector2(12f, 4f),
            BsChemical.Lava(), BsColor.Lava(), BsLayer.Foreground));

        // Joints
        map.joints.Add(new BsJoint(spinnerLeft, null, new BsTransform(),
            BsJointType.Hinge, 100f, 1000000f));
        map.joints.Add(new BsJoint(spinnerRight, null, new BsTransform(),
            BsJointType.Hinge, -100f, 1000000f));

        // Spawns
        map.spawns.Add(new BsSpawn(new BsTransform(-15f, 1f)));
        map.spawns.Add(new BsSpawn(new BsTransform(15f, 1f)));

        MapSystem.current.Load(map);

        MapSystem.current.Spawn(new BsPlayer("Player 1", 1000000f, new BsColor(1f, .5f, 0f)));
        MapSystem.current.Spawn(new BsPlayer("Player 2", 1000000f, new BsColor(0f, .5f, 1f), true));

        /*
        try
        {
            using var writer = new StreamWriter("/home/ian/Downloads/BSMap.txt");
            writer.Write(map.Stringify());
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error while saving map: {e.Message}");
        }
        */
    }
}
