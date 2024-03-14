using Core;
using UnityEngine;
using Utils.Gui;

namespace Gui
{
    public class GuiCf : MonoBehaviour
    {
        // Constants
        public const string PaneId = "cf";

        // Events
        private void Start()
        {
            EventSystem._.OnLoad += OnLoad;
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        private void OnLoad()
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
