using Godot;

namespace Brutalsky.Scripts.Utils;

public static class Vector2Ext
{
    public static float Aspect(this Vector2 _)
    {
        return _.X / _.Y;
    }

    public static Vector2 ForceAspect(this Vector2 _, float aspect, bool minifyOrMaxify = true)
    {
        return (minifyOrMaxify ? _.Aspect() > aspect : _.Aspect() < aspect)
            ? new Vector2(_.X, _.X / aspect)
            : new Vector2(_.Y * aspect, _.Y);
    }
}
