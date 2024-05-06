using System;
using Controllers.Base;
using Core;

namespace Controllers.Gui
{
    public class GuiCfController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "cf";

        // Init functions
        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPmController.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back", () =>
            {
                GuiSystem._.DeactivatePane(PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "save", () => throw new NotImplementedException());
            GuiSystem._.RegisterButton(PaneId, "rset", () => throw new NotImplementedException());
        }
    }
}
