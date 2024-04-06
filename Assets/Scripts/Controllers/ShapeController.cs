using Brutalsky;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class ShapeController : ControllerBase<BsShape>
    {
        // Controller metadata
        public override string Tag => Tags.Shape;
    }
}
