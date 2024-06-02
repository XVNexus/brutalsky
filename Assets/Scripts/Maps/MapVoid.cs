using Data;
using Data.Object;
using Extensions;
using UnityEngine;
using Utils;

namespace Maps
{
    public class MapVoid : MapGenerator
    {
        public override BsMap Generate()
        {
            // Create map
            var result = new BsMap("Void", AuthorBuiltin)
            {
                PlayArea = new Rect(-1250f, -1250f, 2500f, 2500f)
            };

            // Add spawns
            result.Spawns.Add(new BsSpawn(new Vector2(-10f, -10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(10f, -10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(-10f, 10f), 0));
            result.Spawns.Add(new BsSpawn(new Vector2(10f, 10f), 0));

            // TODO: TEMPORARY
            result.Objects.Add(new BsShape("spinner")
            {
                Path = Path.Star(6, 2f, 4f),
                Material = MaterialExt.Metal,
                Dynamic = true
            });
            result.Objects.Add(new BsJoint("spinner-motor", "spinner")
            {
                Type = BsJoint.TypeHinge,
                MotorEnabled = true,
                MotorForce = 1000000f,
                MotorSpeed = 100f
            });
            for (var i = 1; i <= 120; i++)
            {
                result.Objects.Add(new BsMount($"spinner-seat-{i}", "spinner")
                {
                    Position = new Vector2(0f, i * 10f)
                });
            }

            return result;
        }
    }
}
