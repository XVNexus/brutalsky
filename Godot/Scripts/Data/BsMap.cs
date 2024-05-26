using System.Collections.Generic;
using System.Linq;
using Brutalsky.Scripts.Lcs;
using Brutalsky.Scripts.Utils;
using Godot;

namespace Brutalsky.Scripts.Data;

public class BsMap : ILcsDocument
{
    public string Title { get; set; } = "Untitled";
    public string Author { get; set; } = "Anonymous";

    public Rect2 PlayArea { get; set; } = new(-10f, -10f, 20f, 20f);
    public Color BackgroundColor { get; set; } = new(.25f, .25f, .25f);
    public Color LightingColor { get; set; } = new(.8f, .8f, .8f);
    public Vector2 InitialGravity { get; set; } = Vector2.Zero;
    public float InitialAtmosphere { get; set; }
    public float PlayerHealth { get; set; } = 100f;
    public bool AllowDummies { get; set; } = true;

    public List<BsSpawn> Spawns { get; set; } = new();
    public IdList<BsObject> Objects { get; set; } = new();
    public IdList<BsNode> Nodes { get; set; } = new();
    public List<BsLink> Links { get; set; } = new();

    public BsMap(string title, string author)
    {
        Title = title;
        Author = author;
    }

    public BsMap() { }

    public LcsDocument _ToLcs()
    {
        var lines = new List<LcsLine>
        {
            new('!',
                Title,
                Author,
                new object[] { PlayArea.Position.X, PlayArea.Position.Y, PlayArea.Size.X, PlayArea.Size.Y },
                new object[] { BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A },
                new object[] { LightingColor.R, LightingColor.G, LightingColor.B, LightingColor.A },
                new object[] { InitialGravity.X, InitialGravity.Y },
                InitialAtmosphere,
                PlayerHealth,
                AllowDummies
            )
        };
        lines.AddRange(Spawns.Select(LcsLine.Serialize));
        lines.AddRange(Objects.Values.Select(LcsLine.Serialize));
        lines.AddRange(Nodes.Values.Select(LcsLine.Serialize));
        lines.AddRange(Links.Select(LcsLine.Serialize));
        return new LcsDocument(1, new[] { "!$#%^" }, lines.ToArray());
    }

    public void _FromLcs(LcsDocument document)
    {
        if (document.Lines.Length == 0) throw Errors.EmptyLcsDocument();
        var metadata = document.Lines[0].Props;
        if (document.Lines[0].Prefix != '!') throw Errors.InvalidItem("map metadata line", metadata);
        var i = 0;
        Title = (string)metadata[i++];
        Author = (string)metadata[i++];
        var rawPlayArea = (object[])metadata[i++];
        PlayArea = new Rect2((float)rawPlayArea[0], (float)rawPlayArea[1],
            (float)rawPlayArea[2], (float)rawPlayArea[3]);
        var rawBackgroundColor = (object[])metadata[i++];
        BackgroundColor = new Color((float)rawBackgroundColor[0], (float)rawBackgroundColor[1],
            (float)rawBackgroundColor[2], (float)rawBackgroundColor[3]);
        var rawLightingColor = (object[])metadata[i++];
        LightingColor = new Color((float)rawLightingColor[0], (float)rawLightingColor[1],
            (float)rawLightingColor[2], (float)rawLightingColor[3]);
        var rawGravityDirection = (object[])metadata[i++];
        InitialGravity = new Vector2((float)rawGravityDirection[0], (float)rawGravityDirection[1]);
        InitialAtmosphere = (float)metadata[i++];
        PlayerHealth = (float)metadata[i++];
        AllowDummies = (bool)metadata[i++];
        for (var j = 1; j < document.Lines.Length; j++)
        {
            var line = document.Lines[j];
            switch (line.Prefix)
            {
                case '$':
                    Spawns.Add(LcsLine.Deserialize<BsSpawn>(line));
                    break;
                case '#':
                    Objects.Add(LcsLine.Deserialize<BsObject>(line));
                    break;
                case '%':
                    Nodes.Add(LcsLine.Deserialize<BsNode>(line));
                    break;
                case '^':
                    Links.Add(LcsLine.Deserialize<BsLink>(line));
                    break;
                default:
                    throw Errors.InvalidItem("map LCS line prefix", line.Prefix);
            }
        }
    }
}
