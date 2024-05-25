using Brutalsky.Scripts.Data;

namespace Brutalsky.Scripts.Maps;

public abstract class MapGenerator
{
    public const string AuthorBuiltin = "Xveon";
    public const string AuthorGenerated = "Brutalsky";

    public abstract BsMap Generate();
}
