using Brutalsky.Scripts.Data;
using Brutalsky.Scripts.Extensions;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Maps;

public class MapParkourFinish : MapGenerator
{
    public int Number { get; set; }
    public float Difficulty { get; set; }
    public int Seed { get; set; }
    public bool HasNext { get; set; }

    public override BsMap Generate()
    {
        // Create map
        var result = new BsMap("Parkour Finish", AuthorGenerated)
        {
            PlayArea = new Rect2(-15f, -15f, 30f, 30f),
            BackgroundColor = Color.FromHsv(.5f, .75f, .2f),
            LightingColor = new Color(.75f, .75f, .75f),
            InitialGravity = new Vector2(0f, -20f),
            InitialAtmosphere = .5f
        };

        // Add spawns
        result.Spawns.Add(new BsSpawn(new Vector2(-3f, .5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(-1f, .5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(1f, .5f), 0));
        result.Spawns.Add(new BsSpawn(new Vector2(3f, .5f), 0));

        // Add finish platform
        result.Objects.Add(new BsDecal("finish-bg")
        {
            Layer = -1,
            Path = Path.Vector(-5f, 0f, 0, 5f, 0f, 1, 5f, -3f, 0f, -3f, 1, -5f, -3f, -5f, 0f),
            Color = ColorExt.Medkit.MultiplyTint(.5f)
        });
        result.Objects.Add(new BsShape("finish-platform")
        {
            Path = Path.Polygon(-2.5f, -1f, -5f, 0f, 5f, 0f, 2.5f, -1f),
            Material = MaterialExt.Metal,
            Color = ColorExt.Medkit,
            Glow = true
        });
        result.Objects.Add(new BsDecal("finish-beacon")
        {
            Position = new Vector2(0f, 15f),
            Layer = -1,
            Path = Path.Rectangle(10f, 30f),
            Color = ColorExt.Medkit.SetAlpha(.1f)
        });

        return result;
    }
}
