using Controllers.Base;
using Systems;

namespace Controllers.Gui
{
    public class GuiHeController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "he";

        // Init functions
        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPmController.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back", () =>
            {
                GuiSystem._.DeactivatePane(PaneId);
            });
        }
    }
}
