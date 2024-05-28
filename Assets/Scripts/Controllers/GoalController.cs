using Controllers.Base;
using Data.Object;
using Utils;

namespace Controllers
{
    public class GoalController : ControllerBase<BsGoal>
    {
        // Controller metadata
        public override string Tag => Tags.GoalTag;
    }
}
