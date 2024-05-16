using Brutalsky;
using Brutalsky.Addon;
using Brutalsky.Logic;
using Brutalsky.Map;
using Brutalsky.Object;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Joint;
using Utils.Path;

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
                PlayArea = new Rect(-125f, -125f, 250f, 250f)
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-10f, -10f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(10f, -10f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-10f, 10f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(10f, 10f), 0));

            // Add objects
            result.AddObject(new BsShape("spinner")
            {
                Path = PathString.Star(6, 5f, 3f),
                Material = MaterialExt.Stone,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-motor")
            {
                Type = JointType.Hinge,
                MotorEnabled = true,
                MotorForce = 1000f
            }));
            result.AddObject(new BsMount("mount-1")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "spinner",
                Position = new Vector2(0f, 10f)
            });
            result.AddObject(new BsMount("mount-2")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "spinner",
                Position = new Vector2(0f, -10f)
            });

            // Add logic
            result.AddNode(BsNode.Add(2));
            result.AddNode(BsNode.ConstantFloat(500f));
            result.AddNode(BsNode.Multiply(2));
            result.AddLink(new BsLink(4, 1, 0, 0));
            result.AddLink(new BsLink(5, 1, 0, 1));
            result.AddLink(new BsLink(0, 0, 2, 0));
            result.AddLink(new BsLink(1, 0, 2, 1));
            result.AddLink(new BsLink(2, 0, 3, 4));

            return result;
        }

        public static BsMap Brutalsky()
        {
            // Create map
            var result = new BsMap("Brutalsky", Author)
            {
                PlayArea = new Rect(-20f, -10f, 40f, 20f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-16f, 1f), 1));
            result.AddSpawn(new BsSpawn(new Vector2(-14f, 1f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(14f, 1f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(16f, 1f), 1));

            // Add objects
            result.AddObject(new BsShape("wall-left")
            {
                Position = new Vector2(-19.5f, 0f),
                Path = PathString.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-right")
            {
                Position = new Vector2(19.5f, 0f),
                Path = PathString.Rectangle(1f, 20f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-top")
            {
                Position = new Vector2(0f, 10f),
                Path = PathString.Polygon(-15f, 0f, 15f, 0f, 14f, -.5f, -14f, -.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -10f),
                Path = PathString.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("platform-left")
            {
                Position = new Vector2(-19f, 0f),
                Path = PathString.Vector(-1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, 2f, .5f, 0f, 8f, .5f, 0f, 7.5f, 0f, 0f,
                    8f, -.5f, 0f, 2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, -1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("platform-right")
            {
                Position = new Vector2(19f, 0f),
                Path = PathString.Vector(1f, 2.5f, 0f, 0f, 2.5f, 1f, 0f, .5f, -2f, .5f, 0f, -8f, .5f, 0f, -7.5f, 0f, 0f,
                    -8f, -.5f, 0f, -2f, -.5f, 1f, 0f, -.5f, 0f, -2.5f, 0f, 1f, -2.5f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("glue-left")
            {
                Position = new Vector2(-9f, 9.25f),
                Path = PathString.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.AddObject(new BsShape("glue-right")
            {
                Position = new Vector2(9f, 9.25f),
                Path = PathString.Rectangle(10f, .5f),
                Material = MaterialExt.Glue,
                Color = ColorExt.Glue
            });
            result.AddObject(new BsShape("ice-left")
            {
                Position = new Vector2(-14.5f, 9.5f),
                Path = PathString.Polygon(-.5f, .5f, .5f, 0f, .5f, -.5f, -.5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.AddObject(new BsShape("ice-right")
            {
                Position = new Vector2(14.5f, 9.5f),
                Path = PathString.Polygon(.5f, .5f, -.5f, 0f, -.5f, -.5f, .5f, 0f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.AddObject(new BsShape("ice-bottom")
            {
                Position = new Vector2(0f, -9.5f),
                Path = PathString.Polygon(-7f, 0f, -7f, .5f, 0f, 3f, 7f, .5f, 7f, 0f, 0f, 2.5f),
                Material = MaterialExt.Ice,
                Color = ColorExt.Ice
            });
            result.AddObject(new BsShape("rubber-top")
            {
                Position = new Vector2(0f, 9.25f),
                Path = PathString.Polygon(4f, .25f, 4f, -.25f, 0f, -1.25f, -4f, -.25f, -4f, .25f),
                Material = MaterialExt.Rubber,
                Color = ColorExt.Rubber
            });
            result.AddObject(new BsShape("electric-left")
            {
                Position = new Vector2(-11f, 0f),
                Rotation = 45f,
                Path = PathString.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.AddObject(new BsShape("electric-right")
            {
                Position = new Vector2(11f, 0f),
                Rotation = 45f,
                Path = PathString.Square(.7f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Electric
            });
            result.AddObject(new BsDecal("spinner-left-bg")
            {
                Position = new Vector2(-2.5f, 0f),
                Layer = -1,
                Path = PathString.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.AddObject(new BsShape("spinner-left")
            {
                Position = new Vector2(-2.5f, 0f),
                Path = PathString.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-left-motor")
            {
                Type = JointType.Hinge,
                MountAnchor = new Vector2(-2.5f, 0f),
                MotorEnabled = true,
                MotorForce = 200f
            }));
            result.AddObject(new BsDecal("spinner-right-bg")
            {
                Position = new Vector2(2.5f, 0f),
                Layer = -1,
                Path = PathString.Circle(5f),
                Color = Color.white.SetAlpha(.05f)
            });
            result.AddObject(new BsShape("spinner-right")
            {
                Position = new Vector2(2.5f, 0f),
                Rotation = 30f,
                Path = PathString.Star(6, 5f, 3f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-right-motor")
            {
                Type = JointType.Hinge,
                MountAnchor = new Vector2(2.5f, 0f),
                MotorEnabled = true,
                MotorForce = 200f
            }));
            result.AddObject(new BsSensor("spinner-sensor")
            {
                Position = new Vector2(0f, 5f),
                Size = 12f
            });
            result.AddObject(new BsPool("water-left")
            {
                Position = new Vector2(-17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.AddObject(new BsShape("water-left-top")
            {
                Position = new Vector2(-17f, 13.25f),
                Path = PathString.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("water-left-left")
            {
                Position = new Vector2(-19.25f, 11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("water-left-right")
            {
                Position = new Vector2(-14.75f, 11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsPool("water-right")
            {
                Position = new Vector2(17f, 11.25f),
                Rotation = 180f,
                Layer = 1,
                Size = new Vector2(4f, 3.5f),
                Chemical = ChemicalExt.Water,
                Color = ColorExt.Water
            });
            result.AddObject(new BsShape("water-right-top")
            {
                Position = new Vector2(17f, 13.25f),
                Path = PathString.Rectangle(5f, .5f),
                Material = MaterialExt.Medkit,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("water-right-left")
            {
                Position = new Vector2(14.75f, 11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("water-right-right")
            {
                Position = new Vector2(19.25f, 11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Water.MultiplyTint(.5f)
            });
            result.AddObject(new BsPool("lava-left")
            {
                Position = new Vector2(-13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.AddObject(new BsShape("lava-left-bottom")
            {
                Position = new Vector2(-13f, -13.25f),
                Path = PathString.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("lava-left-left")
            {
                Position = new Vector2(-19.25f, -11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("lava-left-right")
            {
                Position = new Vector2(-6.75f, -11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsPool("lava-right")
            {
                Position = new Vector2(13f, -11f),
                Layer = 1,
                Size = new Vector2(12f, 4f),
                Chemical = ChemicalExt.Lava,
                Color = ColorExt.Lava,
                Glow = true
            });
            result.AddObject(new BsShape("lava-right-bottom")
            {
                Position = new Vector2(13f, -13.25f),
                Path = PathString.Rectangle(13f, .5f),
                Material = MaterialExt.Electric,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("lava-right-left")
            {
                Position = new Vector2(6.75f, -11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });
            result.AddObject(new BsShape("lava-right-right")
            {
                Position = new Vector2(19.25f, -11.5f),
                Path = PathString.Rectangle(.5f, 3f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Lava.MultiplyTint(.5f)
            });

            // Add logic
            result.AddNode(BsNode.ConstantFloat(50f));
            result.AddNode(BsNode.ConstantFloat(5000f));
            result.AddNode(BsNode.Multiplexer(2));
            result.AddNode(BsNode.ConstantFloat(1f));
            result.AddNode(BsNode.ConstantFloat(-1f));
            result.AddNode(BsNode.Multiply(2));
            result.AddNode(BsNode.Multiply(2));
            result.AddLink(new BsLink(9, 0, 2, 0));
            result.AddLink(new BsLink(0, 0, 2, 1));
            result.AddLink(new BsLink(1, 0, 2, 2));
            result.AddLink(new BsLink(2, 0, 5, 0));
            result.AddLink(new BsLink(2, 0, 6, 0));
            result.AddLink(new BsLink(3, 0, 5, 1));
            result.AddLink(new BsLink(4, 0, 6, 1));
            result.AddLink(new BsLink(5, 0, 7, 4));
            result.AddLink(new BsLink(6, 0, 8, 4));

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
            result.AddSpawn(new BsSpawn(new Vector2(0f, 5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(5f, 0f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(0f, -5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(-5f, 0f), 0));

            // Add objects
            result.AddObject(new BsDecal("background-top")
            {
                Rotation = 45f,
                Layer = -1,
                Path = PathString.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(1f, .2f, .2f, .05f)
            });
            result.AddObject(new BsShape("wall-top")
            {
                Rotation = 45f,
                Path = PathString.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(1f, .2f, .2f)
            });
            result.AddObject(new BsDecal("background-right")
            {
                Rotation = -45f,
                Layer = -1,
                Path = PathString.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, 1f, .2f, .05f)
            });
            result.AddObject(new BsShape("wall-right")
            {
                Rotation = -45f,
                Path = PathString.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, 1f, .2f)
            });
            result.AddObject(new BsDecal("background-bottom")
            {
                Rotation = -135f,
                Layer = -1,
                Path = PathString.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.2f, 1f, 1f, .05f)
            });
            result.AddObject(new BsShape("wall-bottom")
            {
                Rotation = -135f,
                Path = PathString.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.2f, 1f, 1f)
            });
            result.AddObject(new BsDecal("background-left")
            {
                Rotation = -225f,
                Layer = -1,
                Path = PathString.Vector(0f, 0f, 0f, 0f, 15f, 1f, 15f, 15f, 15f, 0f),
                Color = new Color(.6f, .2f, 1f, .05f)
            });
            result.AddObject(new BsShape("wall-left")
            {
                Rotation = -225f,
                Path = PathString.Vector(0f, 15f, 1f, 15f, 15f, 15f, 0f, 0f, 14f, 0f, 1f, 14f, 14f, 0f, 14f),
                Material = (2f, 2f, 0f, 10f, -10f),
                Color = new Color(.6f, .2f, 1f)
            });
            result.AddObject(new BsShape("spinner")
            {
                Path = PathString.Star(4, 4f, 1f),
                Material = (5f, 0f, 0f, 100f, 0f),
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-motor")
            {
                Type = JointType.Hinge,
                MotorEnabled = true,
                MotorSpeed = 50f,
                MotorForce = 1000000f
            }));
            result.AddObject(new BsShape("spinner-arm-top")
            {
                Position = new Vector2(0f, 10f),
                Path = PathString.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-top-link")
            {
                Type = JointType.Hinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(0f, 10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.AddObject(new BsShape("spinner-arm-right")
            {
                Position = new Vector2(10f, 0f),
                Rotation = -90f,
                Path = PathString.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-right-link")
            {
                Type = JointType.Hinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(10f, 0f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.AddObject(new BsShape("spinner-arm-bottom")
            {
                Position = new Vector2(0f, -10f),
                Rotation = -180f,
                Path = PathString.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-bottom-link")
            {
                Type = JointType.Hinge,
                MountShape = "spinner",
                MountAnchor = new Vector2(0f, -10f),
                MotorEnabled = true,
                MotorSpeed = -1000f,
                MotorForce = 100f
            }));
            result.AddObject(new BsShape("spinner-arm-left")
            {
                Position = new Vector2(-10f, 0f),
                Rotation = -270f,
                Path = PathString.Ngon(3, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true,
                Color = ColorExt.Metal
            }
            .AppendAddon(new BsJoint("spinner-arm-left-link")
            {
                Type = JointType.Hinge,
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
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-7f, -8.5f), 1));
            result.AddSpawn(new BsSpawn(new Vector2(-5f, -8.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(5f, -8.5f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(7f, -8.5f), 1));

            // Add objects
            result.AddObject(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -10f),
                Path = PathString.Polygon(-15f, 0f, -10f, 1f, 10f, 1f, 15f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-left")
            {
                Position = new Vector2(-20f, 10f),
                Path = PathString.Polygon(0f, 0f, 10f, 0f, 5f, -1f, 1f, -1f, 1f, -10f, 0f, -15f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-right")
            {
                Position = new Vector2(20f, 10f),
                Path = PathString.Polygon(0f, 0f, -10f, 0f, -5f, -1f, -1f, -1f, -1f, -10f, 0f, -15f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsDecal("spinner-middle-bg")
            {
                Position = new Vector2(0f, 7f),
                Layer = -1,
                Path = PathString.Circle(4f),
                Color = new Color(.2f, 1f, .2f, .05f)
            });
            result.AddObject(new BsShape("spinner-middle")
            {
                Position = new Vector2(0f, 7f),
                Path = PathString.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.2f, 1f, .2f)
            }
            .AppendAddon(new BsJoint("spinner-middle-motor")
            {
                Type = JointType.Hinge,
                MountAnchor = new Vector2(0f, 7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
            result.AddObject(new BsDecal("spinner-left-bg")
            {
                Position = new Vector2(-17f, -7f),
                Layer = -1,
                Path = PathString.Circle(4f),
                Color = new Color(1f, .2f, .2f, .05f)
            });
            result.AddObject(new BsShape("spinner-left")
            {
                Position = new Vector2(-17f, -7f),
                Path = PathString.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(1f, .2f, .2f)
            }
            .AppendAddon(new BsJoint("spinner-left-motor")
            {
                Type = JointType.Hinge,
                MountAnchor = new Vector2(-17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));
            result.AddObject(new BsDecal("spinner-right-bg")
            {
                Position = new Vector2(17f, -7f),
                Layer = -1,
                Path = PathString.Circle(4f),
                Color = new Color(.2f, .2f, 1f, .05f)
            });
            result.AddObject(new BsShape("spinner-right")
            {
                Position = new Vector2(17f, -7f),
                Path = PathString.Star(3, 4f, 1f),
                Material = (5f, 0f, 20f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.2f, .2f, 1f)
            }
            .AppendAddon(new BsJoint("spinner-right-motor")
            {
                Type = JointType.Hinge,
                MountAnchor = new Vector2(17f, -7f),
                MotorEnabled = true,
                MotorForce = 10000f
            }));

            // Add logic
            result.AddNode(BsNode.GameTime());
            result.AddNode(BsNode.ConstantFloat(Mathf.PI));
            result.AddNode(BsNode.Multiply(2));
            result.AddNode(BsNode.Sin());
            result.AddNode(BsNode.ConstantFloat(1000f));
            result.AddNode(BsNode.Multiply(2));
            result.AddNode(BsNode.Clock(50));
            result.AddNode(BsNode.ChangeListener());
            result.AddNode(BsNode.RandomInt(0, 2));
            result.AddNode(BsNode.Demultiplexer(3));
            result.AddLink(new BsLink(0, 0, 2, 0));
            result.AddLink(new BsLink(1, 0, 2, 1));
            result.AddLink(new BsLink(2, 0, 3, 0));
            result.AddLink(new BsLink(3, 0, 5, 0));
            result.AddLink(new BsLink(4, 0, 5, 1));
            result.AddLink(new BsLink(5, 0, 9, 1));
            result.AddLink(new BsLink(6, 0, 7, 0));
            result.AddLink(new BsLink(7, 0, 8, 0));
            result.AddLink(new BsLink(8, 0, 9, 0));
            result.AddLink(new BsLink(9, 0, 10, 4));
            result.AddLink(new BsLink(9, 1, 11, 4));
            result.AddLink(new BsLink(9, 2, 12, 4));

            return result;
        }

        public static BsMap Racetrack()
        {
            // Create map
            var result = new BsMap("Racetrack", Author)
            {
                PlayArea = new Rect(-1250f, 0f, 2500f, 250f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f
            };

            // Add spawns
            result.AddSpawn(new BsSpawn(new Vector2(-5f, 11f), 1));
            result.AddSpawn(new BsSpawn(new Vector2(-3f, 11f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(3f, 11f), 0));
            result.AddSpawn(new BsSpawn(new Vector2(5f, 11f), 1));

            // Add objects
            result.AddObject(new BsShape("wall-bottom")
            {
                Position = new Vector2(0f, -5f),
                Path = PathString.Rectangle(2500f, 12f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-left")
            {
                Position = new Vector2(-1255f, 10f),
                Path = PathString.Rectangle(12f, 42f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("wall-right")
            {
                Position = new Vector2(1255f, 10f),
                Path = PathString.Rectangle(12f, 42f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("corner-bl")
            {
                Position = new Vector2(-1249f, 1f),
                Path = PathString.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, 20f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("corner-br")
            {
                Position = new Vector2(1249f, 1f),
                Path = PathString.Vector(0f, 0f, 0f, 0f, 20f, 1f, 0f, 0f, -20f, 0f),
                Material = MaterialExt.Stone,
                Color = ColorExt.Stone
            });
            result.AddObject(new BsShape("car-body")
            {
                Position = new Vector2(0f, 10f),
                Path = PathString.Vector(-6f, .5f, 0f, -3f, .5f, 0f, -2.5f, 1f, 0f, -1.6f, 1f, 1f, -1.6f, .5f, -1f, .4f,
                    1f, -.4f, .4f, -.4f, 1f, 0f, .4f, 1f, 1f, .4f, .5f, 1f, .4f, 1f, 1.6f, .4f, 1.6f, 1f, 0f, 2.5f, 1f,
                    0f, 3f, .5f, 0f, 6f, .5f, 0f, 3f, -1f, 0f, -3f, -1f),
                Material = MaterialExt.Stone,
                Dynamic = true,
                Color = Color.white
            });
            result.AddObject(new BsShape("car-wheel-1-axle")
            {
                Position = new Vector2(-6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = PathString.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-1-anchor")
            {
                Type = JointType.Slider,
                MountShape = "car-body",
                MountAnchor = new Vector2(-6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-1-spring")
            {
                Type = JointType.Spring,
                MountShape = "car-body",
                MountAnchor = new Vector2(-6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.AddObject(new BsShape("car-wheel-1")
            {
                Position = new Vector2(-6f, 7f),
                Path = PathString.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-1-motor")
            {
                Type = JointType.Hinge,
                MountShape = "car-wheel-1-axle",
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.AddObject(new BsShape("car-wheel-2-axle")
            {
                Position = new Vector2(6f, 7f),
                Rotation = 45f,
                Layer = 1,
                Path = PathString.Square(1f),
                Material = (0f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = Color.white
            }
            .AppendAddon(new BsJoint("car-wheel-2-anchor")
            {
                Type = JointType.Slider,
                MountShape = "car-body",
                MountAnchor = new Vector2(6f, 0f),
                AngleValue = 90f
            })
            .AppendAddon(new BsJoint("car-wheel-2-spring")
            {
                Type = JointType.Spring,
                MountShape = "car-body",
                MountAnchor = new Vector2(6f, 0f),
                DistanceValue = 3f,
                DampingRatio = .3f,
                DampingFrequency = 15f
            }));
            result.AddObject(new BsShape("car-wheel-2")
            {
                Position = new Vector2(6f, 7f),
                Path = PathString.Circle(3f),
                Material = (10f, 0f, 0f, 1f, 0f),
                Dynamic = true,
                Color = new Color(.15f, .15f, .15f)
            }
            .AppendAddon(new BsJoint("car-wheel-2-motor")
            {
                Type = JointType.Hinge,
                MountShape = "car-wheel-2-axle",
                MotorEnabled = true,
                MotorForce = 2000f
            }));
            result.AddObject(new BsMount("car-seat-1")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "car-body",
                Position = new Vector2(1f, 1f)
            });
            result.AddObject(new BsMount("car-seat-2")
            {
                ParentTag = Tags.ShapePrefix,
                ParentId = "car-body",
                Position = new Vector2(-1f, 1f)
            });

            // Add logic
            result.AddNode(BsNode.ConstantFloat(15000f));
            result.AddNode(BsNode.Multiply(2));
            result.AddLink(new BsLink(8, 1, 1, 0));
            result.AddLink(new BsLink(0, 0, 1, 1));
            result.AddLink(new BsLink(1, 0, 4, 4));
            result.AddLink(new BsLink(1, 0, 7, 4));

            return result;
        }
    }
}
