using Godot;

namespace Brutalsky.Scripts.Extensions;

public static class Rect2Ext
{
    public static Vector2 Corner(this Rect2 _)
    {
        return _.Position + _.Size;
    }

    public static float Aspect(this Rect2 _)
    {
        return _.Size.Aspect();
    }

    public static Rect2 ForceAspect(this Rect2 _, float aspect, bool minifyOrMaxify = true)
    {
        var newSize = _.Size.ForceAspect(aspect, minifyOrMaxify);
        return new Rect2(new Vector2(_.Position.X - (newSize.X - _.Size.X) * .5f,
            _.Position.Y - (newSize.Y - _.Size.Y) * .5f), newSize);
    }

    public static Rect2 Resize(this Rect2 _, Vector2 newSize)
    {
        return new Rect2(_.Position - (newSize - _.Size) * .5f, newSize);
    }

    public static Rect2 Expand(this Rect2 _, float growth)
    {
        return new Rect2(_.Position.X - growth, _.Position.Y - growth, _.Size.X + growth * 2f, _.Size.Y + growth * 2f);
    }
}
