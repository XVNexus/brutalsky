using Controllers.Base;
using Data.Object;
using Utils.Constants;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Tag => Tags.PlayerTag;
    }
}
