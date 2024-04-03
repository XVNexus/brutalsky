using Core;
using Utils.Gui;

namespace Gui
{
    public class GuiCf : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "cf";

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPm.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back");
        }

        // Event functions
        private void OnGuiAction(GuiAction action, string paneId, string itemId)
        {
            if (paneId != PaneId) return;
            switch (itemId)
            {
                case "back":
                    OnButtonPressBack();
                    break;
            }
        }

        private void OnButtonPressBack()
        {
            GuiSystem._.DeactivatePane(PaneId);
        }
    }
}
