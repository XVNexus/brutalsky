using Data;
using Data.Map;
using UnityEngine;

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

            return result;
        }
    }
}
