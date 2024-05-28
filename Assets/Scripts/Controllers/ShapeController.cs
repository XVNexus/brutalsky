using Controllers.Base;
using Data.Object;
using Utils;

namespace Controllers
{
    public class ShapeController : ControllerBase<BsShape>
    {
        // Controller metadata
        public override string Tag => Tags.ShapeTag;
    }
}
