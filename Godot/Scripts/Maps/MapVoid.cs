using Brutalsky.Scripts.Data;
using Godot;

namespace Brutalsky.Scripts.Maps;

public class MapVoid : MapGenerator
{
    public override BsMap Generate()
    {
        // Create map
        var result = new BsMap("Void", AuthorBuiltin)
        {
            PlayArea = new Rect2(-125f, -125f, 250f, 250f)
        };

        // Add spawns
        result.Spawns.Add(new BsSpawn(new Vector2(-10f, -10f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(10f, -10f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(-10f, 10f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(10f, 10f), 0));

        return result;
    }
}
