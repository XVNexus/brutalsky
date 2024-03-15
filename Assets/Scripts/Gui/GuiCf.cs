using Core;
using Utils.Gui;

namespace Gui
{
    public class GuiCf : BsBehavior
    {
        public const string PaneId = "cf";

        protected override void OnStart()
        {
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPm.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back");
        }

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
