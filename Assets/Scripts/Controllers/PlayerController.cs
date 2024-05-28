using Controllers.Base;
using Data;
using Data.Object;
using Utils;

namespace Controllers
{
    public class PlayerController : ControllerBase<BsPlayer>
    {
        // Controller metadata
        public override string Tag => Tags.PlayerTag;
    }
}
