using Data;
using Data.Addon;
using Data.Logic;
using Data.Map;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils.Constants;

namespace Utils.Map
{
    public static class MapBuiltins
    {
        public const string Author = "Xveon";

        public static BsMap Void()
        {
            // Create map
            var result = new BsMap("Void", Author)
            {
                PlayArea = new Rect(-1250f, -1250f, 2500f, 2500f)
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-10f, -10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(10f, -10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(-10f, 10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(10f, 10f), 0));

            return result;
        }

        public static BsMap Brutalsky()
        {
            // Create map
            var result = new BsMap("Brutalsky", Author)
            {
                PlayArea = new Rect(-20f, -10f, 40f, 20f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-16f, 1f), 1));
            result.Spawns.Add(new BsSpawn(new Vector2(-14f, 1f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(14f, 1f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(16f, 1f), 1));

            // Add objects
            result.Objects.Add(new BsShape("wall-left")
            {
                Position = new Vector2(-19.5f, 0f),
                Path = Path.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Position = new Vector2(19.5f, 0f),
                Path = Path.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-top")
            {
                Position = new Vector2(0f, 10f),
                Path = Path.Polygon(-15f, 0f, 15f, 0f, 14f, -.5f, -14f, -.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -10f),
                Path = Path.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("platform-left")
            {
                Position = new Vector2(-19f, 0f),
                Path = Path.Vector(-1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, 2f, .5f, 0f, 8f, .5f, 0f, 7.5f, 0f, 0f,
                    8f, -.5f, 0f, 2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, -1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("platform-right")
            {
                Position = new Vector2(19f, 0f),
                Path = Path.Vector(1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, -2f, .5f, 0f, -8f, .5f, 0f, -7.5f, 0f, 0f,
                    -8f, -.5f, 0f, -2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, 1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("glue-left")
            {
                Position = new Vector2(-9f, 9.25f),
                Path = Path.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.Objects.Add(new BsShape("glue-right")
            {
                Position = new Vector2(9f, 9.25f),
                Path = Path.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.Objects.Add(new BsShape("ice-left")
            {
                Position = new Vector2(-14.5f, 9.5f),
                Path = Path.Polygon(-.5f, .5f, .5f, 0f, .5f, -.5f, -.5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("ice-right")
            {
                Position = new Vector2(14.5f, 9.5f),
                Path = Path.Polygon(.5f, .5f, -.5f, 0f, -.5f, -.5f, .5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("ice-bottom")
            {
                Position = new Vector2(0f, -9.5f),
                Path = Path.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f, 0f, 2.5f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.Objects.Add(new BsShape("rubber-top")
            {
                Position = new Vector2(0f, 9.25f),
                Path = Path.Polygon(4f, .25f, 4f, -.25f, 0f, -1.25f, -4f, -.25f, -4f, .25f),
                Material = MaterialExt.Rubber,
                Color = ColorExt.Rubber
            });
            result.Objects.Add(new BsShape("electric-left")
            {
                Position = new Vector2(-11f, 0f),
                Rotation = 45f,
                Path = Path.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.Objects.Add(new BsShape("electric-right")
            {
                Position = new Vector2(11f, 0f),
                Rotation = 45f,
                Path = Path.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.Objects.Add(new BsDecal("spinner-left-bg")
            {
                Position = new Vector2(-2.5f, 0f),
                Layer = -1,
                Path = Path.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.Objects.Add(new BsShape("spinner-left")
            {
                Position = new Vector2(-2.5f, 0f),
                Path = Path.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-left-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(-2.5f, 0f),
                MotorEnabled = true,
                MotorForce = 500f
            }));
            result.Objects.Add(new BsDecal("spinner-right-bg")
            {
                Position = new Vector2(2.5f, 0f),
                Layer = -1,
                Path = Path.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.Objects.Add(new BsShape("spinner-right")
            {
                Position = new Vector2(2.5f, 0f),
                Rotation = 30f,
                Path = Path.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-right-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(2.5f, 0f),
                MotorEnabled = true,
                MotorForce = 500f
            }));
            result.Objects.Add(new BsSensor("spinner-sensor")
            {
                Position = new Vector2(0f, 5f),
                Size = new Vector2(10f, 10f)
            });
            result.Objects.Add(new BsPool("water-left")
            {
                Position = new Vector2(-17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.Objects.Add(new BsShape("water-left-top")
            {
                Position = new Vector2(-17f, 13.25f),
                Path = Path.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-left-left")
            {
                Position = new Vector2(-19.25f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-left-right")
            {
                Position = new Vector2(-14.75f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("water-right")
            {
                Position = new Vector2(17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.Objects.Add(new BsShape("water-right-top")
            {
                Position = new Vector2(17f, 13.25f),
                Path = Path.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-right-left")
            {
                Position = new Vector2(14.75f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("water-right-right")
            {
                Position = new Vector2(19.25f, 11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("lava-left")
            {
                Position = new Vector2(-13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.Objects.Add(new BsShape("lava-left-bottom")
            {
                Position = new Vector2(-13f, -13.25f),
                Path = Path.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-left-left")
            {
                Position = new Vector2(-19.25f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-left-right")
            {
                Position = new Vector2(-6.75f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsPool("lava-right")
            {
                Position = new Vector2(13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.Objects.Add(new BsShape("lava-right-bottom")
            {
                Position = new Vector2(13f, -13.25f),
                Path = Path.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-right-left")
            {
                Position = new Vector2(6.75f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.Objects.Add(new BsShape("lava-right-right")
            {
                Position = new Vector2(19.25f, -11.5f),
                Path = Path.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });

            // Add logic
            result.Nodes.Add(BsNode.ConstantFloat("speed-idle", 50f));
            result.Nodes.Add(BsNode.ConstantFloat("speed-active", 5000f));
            result.Nodes.Add(BsNode.Multiplexer("speed-selector", 2));
            result.Nodes.Add(BsNode.ConstantFloat("direction-positive", 1f));
            result.Nodes.Add(BsNode.ConstantFloat("direction-negative", -1f));
            result.Nodes.Add(BsNode.Multiply("motor-left-controller", 2));
            result.Nodes.Add(BsNode.Multiply("motor-right-controller", 2));
            result.Links.Add(new BsLink("spinner-sensor", 0, "speed-selector", 0));
            result.Links.Add(new BsLink("speed-idle", 0, "speed-selector", 1));
            result.Links.Add(new BsLink("speed-active", 0, "speed-selector", 2));
            result.Links.Add(new BsLink("speed-selector", 0, "motor-left-controller", 0));
            result.Links.Add(new BsLink("speed-selector", 0, "motor-right-controller", 0));
            result.Links.Add(new BsLink("direction-positive", 0, "motor-left-controller", 1));
            result.Links.Add(new BsLink("direction-negative", 0, "motor-right-controller", 1));
            result.Links.Add(new BsLink("motor-left-controller", 0, "spinner-left-motor", 4));
            result.Links.Add(new BsLink("motor-right-controller", 0, "spinner-right-motor", 4));

            return result;
        }

        public static BsMap Doomring()
        {
            // Create map
            var result = new BsMap("Doomring", Author)
            {
                PlayArea = new Rect(-15f, -15f, 30f, 30f)
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(0f, 5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(5f, 0f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(0f, -5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(-5f, 0f), 0));

            // Add objects
            result.Objects.Add(new BsDecal("background-top")
            {
                Rotation = 45f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(1f, .2f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("wall-top")
            {
                Rotation = 45f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(1f, .2f, .2f)
            });
            result.Objects.Add(new BsDecal("background-right")
            {
                Rotation = -45f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, 1f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Rotation = -45f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, 1f, .2f)
            });
            result.Objects.Add(new BsDecal("background-bottom")
            {
                Rotation = -135f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.2f, 1f, 1f, .05f)
            });
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Rotation = -135f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.2f, 1f, 1f)
            });
            result.Objects.Add(new BsDecal("background-left")
            {
                Rotation = -225f,
                Layer = -1,
                Path = Path.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, .2f, 1f, .05f)
            });
            result.Objects.Add(new BsShape("wall-left")
            {
                Rotation = -225f,
                Path = Path.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, .2f, 1f)
            });
            result.Objects.Add(new BsShape("spinner")
            {
                Path = Path.Star(4, 4f, 1f),
                Material = (5f, 0f, 0f, 100f, 0f),
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-motor")
            {
                Type = BsJoint.TypeHinge,
                MotorEnabled = true,
                MotorSpeed = 50f,
                MotorForce = 1000000f
            }));
            result.Objects.Add(new BsShape("spinner-arm-top")
            {
                Position = new Vector2(0f, 10f),
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-top-link")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(0f, 10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-right")
            {
                Position = new Vector2(10f, 0f),
                Rotation = -90f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-right-link")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(10f, 0f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-bottom")
            {
                Position = new Vector2(0f, -10f),
                Rotation = -180f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-bottom-link")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(0f, -10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.Objects.Add(new BsShape("spinner-arm-left")
            {
                Position = new Vector2(-10f, 0f),
                Rotation = -270f,
                Path = Path.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-left-link")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(-10f, 0f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));

            return result;
        }

        public static BsMap Tossup()
        {
            // Create map
            var result = new BsMap("Tossup", Author)
            {
                PlayArea = new Rect(-20f, -10f, 40f, 20f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-7f, -8.5f), 1));
            result.Spawns.Add(new BsSpawn(new Vector2(-5f, -8.5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(5f, -8.5f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(7f, -8.5f), 1));

            // Add objects
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -10f),
                Path = Path.Polygon(-15f, 0f, -10f, 1f, 10f, 1f, 15f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-left")
            {
                Position = new Vector2(-20f, 10f),
                Path = Path.Polygon(0f, 0f, 10f, 0f, 5f, -1f, 1f, -1f, 1f, -10f, 0f, -15f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Position = new Vector2(20f, 10f),
                Path = Path.Polygon(0f, 0f, -10f, 0f, -5f, -1f, -1f, -1f, -1f, -10f, 0f, -15f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsDecal("spinner-left-bg")
            {
                Position = new Vector2(-17f, -7f),
                Layer = -1,
                Path = Path.Circle(4f),
                Color = new Color(1f, .2f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("spinner-left")
            {
                Position = new Vector2(-17f, -7f),
                Path = Path.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(1f, .2f, .2f)
            }
            .AppendAddon(new BsJoint("spinner-left-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(-17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
            result.Objects.Add(new BsDecal("spinner-middle-bg")
            {
                Position = new Vector2(0f, 7f),
                Layer = -1,
                Path = Path.Circle(4f),
                Color = new Color(.2f, 1f, .2f, .05f)
            });
            result.Objects.Add(new BsShape("spinner-middle")
            {
                Position = new Vector2(0f, 7f),
                Path = Path.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.2f, 1f, .2f)
            }
            .AppendAddon(new BsJoint("spinner-middle-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(0f, 7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
            result.Objects.Add(new BsDecal("spinner-right-bg")
            {
                Position = new Vector2(17f, -7f),
                Layer = -1,
                Path = Path.Circle(4f),
                Color = new Color(.2f, .2f, 1f, .05f)
            });
            result.Objects.Add(new BsShape("spinner-right")
            {
                Position = new Vector2(17f, -7f),
                Path = Path.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.2f, .2f, 1f)
            }
            .AppendAddon(new BsJoint("spinner-right-motor")
            {
                Type = BsJoint.TypeHinge,
                MountAnchor = new Vector2(17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));

            // Add logic
            result.Nodes.Add(BsNode.Timer("map-timer"));
            result.Nodes.Add(BsNode.ConstantFloat("pi", Mathf.PI));
            result.Nodes.Add(BsNode.Multiply("timer-scaled", 2));
            result.Nodes.Add(BsNode.Sin("speed-scale"));
            result.Nodes.Add(BsNode.ConstantFloat("speed-max", 1000f));
            result.Nodes.Add(BsNode.Multiply("speed-output", 2));
            result.Nodes.Add(BsNode.Clock("spinner-timer", 50));
            result.Nodes.Add(BsNode.Monostable("spinner-switcher"));
            result.Nodes.Add(BsNode.RandomInt("spinner-selector", 0, 2));
            result.Nodes.Add(BsNode.Demultiplexer("spinner-controller", 3));
            result.Links.Add(new BsLink("map-timer", 0, "timer-scaled", 0));
            result.Links.Add(new BsLink("pi", 0, "timer-scaled", 1));
            result.Links.Add(new BsLink("timer-scaled", 0, "speed-scale", 0));
            result.Links.Add(new BsLink("speed-scale", 0, "speed-output", 0));
            result.Links.Add(new BsLink("speed-max", 0, "speed-output", 1));
            result.Links.Add(new BsLink("speed-output", 0, "spinner-controller", 1));
            result.Links.Add(new BsLink("spinner-timer", 0, "spinner-switcher", 0));
            result.Links.Add(new BsLink("spinner-switcher", 0, "spinner-selector", 0));
            result.Links.Add(new BsLink("spinner-selector", 0, "spinner-controller", 0));
            result.Links.Add(new BsLink("spinner-controller", 0, "spinner-left-motor", 4));
            result.Links.Add(new BsLink("spinner-controller", 1, "spinner-middle-motor", 4));
            result.Links.Add(new BsLink("spinner-controller", 2, "spinner-right-motor", 4));

            return result;
        }

        public static BsMap Racetrack()
        {
            // Create map
            var result = new BsMap("Racetrack", Author)
            {
                PlayArea = new Rect(-1250f, 0f, 2500f, 250f),
                GravityDirection = BsMap.DirectionDown,
                GravityStrength = 20f,
                AirResistance = .5f
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-5f, 11f), 1));
            result.Spawns.Add(new BsSpawn(new Vector2(-3f, 11f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(3f, 11f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(5f, 11f), 1));

            // Add objects
            result.Objects.Add(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -5f),
                Path = Path.Rectangle(2500f, 12f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-left")
            {
                Position = new Vector2(-1255f, 10f),
                Path = Path.Rectangle(12f, 42f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("wall-right")
            {
                Position = new Vector2(1255f, 10f),
                Path = Path.Rectangle(12f, 42f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("corner-bl")
            {
                Position = new Vector2(-1249f, 1f),
                Path = Path.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, 20f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("corner-br")
            {
                Position = new Vector2(1249f, 1f),
                Path = Path.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, -20f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.Objects.Add(new BsShape("car-body")
            {
                Position = new Vector2(0f, 10f),
                Path = Path.Vector(-6f, .5f, 0f, -3f, .5f, 0f, -2.5f, 1f, 0f, -1.6f, 1f, 1f, -1.6f, .5f, -1f, .4f,
                    1f, -.4f, .4f, -.4f, 1f, 0f, .4f, 1f, 1f, .4f, .5f, 1f, .4f, 1f, 1.6f, .4f, 1.6f, 1f, 0f, 2.5f, 1f,
                    0f, 3f, .5f, 0f, 6f, .5f, 0f, 3f, -1f, 0f, -3f, -1f),
                Material = MaterialExt.Stone,
                Dynamic = true,
                Color = Color.white
            });
            result.Objects.Add(new BsShape("car-wheel-1-axle")
            {
                Position = new Vector2(-6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = Path.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-1-anchor")
            {
                Type = BsJoint.TypeSlider,
                MountShape = "car-body",
                MountAnchor = new Vector2(-6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-1-spring")
            {
                Type = BsJoint.TypeSpring,
                MountShape = "car-body",
                MountAnchor = new Vector2(-6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.Objects.Add(new BsShape("car-wheel-1")
            {
                Position = new Vector2(-6f, 7f),
                Path = Path.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-1-motor")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "car-wheel-1-axle",
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.Objects.Add(new BsShape("car-wheel-2-axle")
            {
                Position = new Vector2(6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = Path.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-2-anchor")
            {
                Type = BsJoint.TypeSlider,
                MountShape = "car-body",
                MountAnchor = new Vector2(6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-2-spring")
            {
                Type = BsJoint.TypeSpring,
                MountShape = "car-body",
                MountAnchor = new Vector2(6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.Objects.Add(new BsShape("car-wheel-2")
            {
                Position = new Vector2(6f, 7f),
                Path = Path.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-2-motor")
            {
                Type = BsJoint.TypeHinge,
                MountShape = "car-wheel-2-axle",
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.Objects.Add(new BsMount("car-seat-1")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "car-body",
                Position = new Vector2(1f, 1f)
            });
            result.Objects.Add(new BsMount("car-seat-2")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "car-body",
                Position = new Vector2(-1f, 1f)
            });

            // Add logic
            result.Nodes.Add(BsNode.ConstantFloat("wheel-speed", 15000f));
            result.Nodes.Add(BsNode.Multiply("wheel-output", 2));
            result.Links.Add(new BsLink("car-seat-1", 1, "wheel-output", 0));
            result.Links.Add(new BsLink("wheel-speed", 0, "wheel-output", 1));
            result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-1-motor", 4));
            result.Links.Add(new BsLink("wheel-output", 0, "car-wheel-2-motor", 4));

            return result;
        }
    }
}
