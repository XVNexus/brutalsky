using System;
using System.IO;
using Brutalsky;
using Brutalsky.Joint;
using Brutalsky.Object;
using Brutalsky.Pool;
using Brutalsky.Shape;
using Core;
using Serializable;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Testing : MonoBehaviour
{
    public void Start()
    {
        var map = new BsMap("Brutalsky", "xveon")
        {
            size = new Vector2(40f, 20f),
            lighting = new BsColor(1f, 1f, 1f, .8f)
        };

        // Decorative shapes
        map.Add(new BsShape("spinner-left-bg", new BsTransform(-2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(.3f, .3f, .3f), BsLayer.Background, false));
        map.Add(new BsShape("spinner-right-bg", new BsTransform(2f, 0f), BsPath.Circle(5f),
            BsMaterial.Stone(), new BsColor(.3f, .3f, .3f), BsLayer.Background, false));

        // Structure shapes
        map.Add(new BsShape("wall-left", new BsTransform(-20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("wall-right", new BsTransform(20f, 0f), BsPath.Rectangle(2f, 22f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("wall-top", new BsTransform(0f, 10f), BsPath.Path("-15 1 L 15 1 L 15 0 L 14 -.5 L -14 -.5 L -15 0"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("wall-bottom", new BsTransform(0f, -10f), BsPath.Path("-7 -1 L -7 .5 L 0 3 L 7 .5 L 7 -1"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("platform-left", new BsTransform(-19f, 0f), BsPath.Path("-1 2.5 L 0 2.5 C 0 .5 2 .5 L 8 .5 L 7.5 0 L 8 -.5 L 2 -.5 C 0 -.5 0 -2.5 L -1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("platform-right", new BsTransform(19f, 0f), BsPath.Path("1 2.5 L 0 2.5 C 0 .5 -2 .5 L -8 .5 L -7.5 0 L -8 -.5 L -2 -.5 C 0 -.5 0 -2.5 L 1 -2.5"),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tl-wall-top", new BsTransform(-17f, 13.25f), BsPath.Rectangle(5f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tl-wall-left", new BsTransform(-19.25f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tl-wall-right", new BsTransform(-14.75f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tr-wall-top", new BsTransform(17f, 13.25f), BsPath.Rectangle(5f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tr-wall-left", new BsTransform(14.75f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-tr-wall-right", new BsTransform(19.25f, 12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-bl-wall-bottom", new BsTransform(-13f, -13.25f), BsPath.Rectangle(13f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-bl-wall-left", new BsTransform(-19.25f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-bl-wall-right", new BsTransform(-6.75f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-br-wall-bottom", new BsTransform(13f, -13.25f), BsPath.Rectangle(13f, .5f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-br-wall-left", new BsTransform(6.75f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));
        map.Add(new BsShape("pool-br-wall-right", new BsTransform(19.25f, -12f), BsPath.Rectangle(.5f, 2f),
            BsMaterial.Stone(), BsColor.Stone()));

        // Interactive shapes
        map.Add(new BsShape("glue-left", new BsTransform(-9f, 9.25f), BsPath.Rectangle(10f, .5f), 
            BsMaterial.Glue(), BsColor.Glue()));
        map.Add(new BsShape("glue-right", new BsTransform(9f, 9.25f), BsPath.Rectangle(10f, .5f),
            BsMaterial.Glue(), BsColor.Glue()));
        map.Add(new BsShape("ice-left", new BsTransform(-14.5f, 9.5f), BsPath.Path("-.5 .5 L .5 0 L .5 -.5 L -.5 0"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.Add(new BsShape("ice-right", new BsTransform(14.5f, 9.5f), BsPath.Path(".5 .5 L -.5 0 L -.5 -.5 L .5 0"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.Add(new BsShape("ice-bottom", new BsTransform(0f, -9.5f), BsPath.Path("-7 0 L -7 .5 L 0 3 L 7 .5 L 7 0 L 0 2.5"),
            BsMaterial.Ice(), BsColor.Ice()));
        map.Add(new BsShape("rubber-top", new BsTransform(0f, 9.25f), BsPath.Path("4 .25 L 4 -.25 L 0 -1.25 L -4 -.25 L -4 .25"),
            BsMaterial.Rubber(), BsColor.Rubber()));
        map.Add(new BsShape("electric-left", new BsTransform(-11f, 0f, 45f), BsPath.Square(.7f),
            BsMaterial.Electric(), BsColor.Electric()));
        map.Add(new BsShape("electric-right", new BsTransform(11f, 0f, 45f), BsPath.Square(.7f),
            BsMaterial.Electric(), BsColor.Electric()));

        // Dynamic shapes
        map.Add(new BsShape("spinner-left", new BsTransform(-2f, 0f), BsPath.Star(6, 4.9f, 2.9f),
            BsMaterial.Metal(true), BsColor.Metal()));
        map.Add(new BsShape("spinner-right", new BsTransform(2f, 0f, 30f), BsPath.Star(6, 4.9f, 2.9f),
            BsMaterial.Metal(true), BsColor.Metal()));

        // Pools
        map.Add(new BsPool("water-left", new BsTransform(-17f, 11.25f, 180f), new Vector2(4f, 3.5f),
            BsChemical.Water(), BsColor.Water(), BsLayer.Foreground));
        map.Add(new BsPool("water-right", new BsTransform(17f, 11.25f, 180f), new Vector2(4f, 3.5f),
            BsChemical.Water(), BsColor.Water(), BsLayer.Foreground));
        map.Add(new BsPool("lava-left", new BsTransform(-13f, -11f), new Vector2(12f, 4f),
            BsChemical.Lava(), BsColor.Lava(), BsLayer.Foreground));
        map.Add(new BsPool("lava-right", new BsTransform(13f, -11f), new Vector2(12f, 4f),
            BsChemical.Lava(), BsColor.Lava(), BsLayer.Foreground));

        // Joints
        map.Add(new BsJointHinge("spinner-left-motor", new BsTransform(),
            "spinner-left", "",
            false, BsJointStrength.Unbreakable(), BsJointMotor.Powered(100f), BsJointLimits.Unlimited()));
        map.Add(new BsJointHinge("spinner-right-motor", new BsTransform(),
            "spinner-right", "",
            false, BsJointStrength.Unbreakable(), BsJointMotor.Powered(-100f), BsJointLimits.Unlimited()));

        // Spawns
        map.Add(new BsSpawn("spawn-left", new BsTransform(-15f, 1f)));
        map.Add(new BsSpawn("spawn-right", new BsTransform(15f, 1f)));

        // Write map to string and save to file
        var serializer = new SerializerBuilder().Build();
        var mapString = serializer.Serialize(SrzMap.Simplify(map));
        try
        {
            using var writer = new StreamWriter("/home/ian/Downloads/output.yaml");
            writer.Write(mapString);
        }
        catch (IOException ex)
        {
            Debug.Log("IO ERROR");
        }

        // Read map from string and load
        var deserializer = new DeserializerBuilder().Build();
        var mapParsed = deserializer.Deserialize<SrzMap>(mapString).Expand();
        MapSystem.current.Load(mapParsed);
        MapSystem.current.Spawn(new BsPlayer("player-1", 100f, new BsColor(1f, .5f, 0f)));
        MapSystem.current.Spawn(new BsPlayer("player-2", 100f, new BsColor(0f, .5f, 1f), true));
    }
}
