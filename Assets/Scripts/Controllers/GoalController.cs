using Brutalsky.Object;
using Controllers.Base;
using Utils.Constants;

namespace Controllers
{
    public class GoalController : ControllerBase<BsGoal>
    {
        // Controller metadata
        public override string Tag => Tags.GoalTag;
    }
}
