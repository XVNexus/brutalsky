using Controllers.Base;
using Data.Object;
using Utils.Constants;

namespace Controllers
{
    public class ShapeController : ControllerBase<BsShape>
    {
        // Controller metadata
        public override string Tag => Tags.ShapeTag;
    }
}
