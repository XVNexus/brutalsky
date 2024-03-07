using Core;
using UnityEngine;

namespace Controllers.Gui
{
    public class GuiCfController : MonoBehaviour
    {
        // Constants
        public const string PaneId = "cf";

        // References
        private GuiController cGuiController;

        // Events
        private void Start()
        {
            EventSystem.current.OnGuiLoad += OnGuiLoad;
            EventSystem.current.OnGuiAction += OnGuiAction;

            cGuiController = GetComponent<GuiController>();
        }

        private void OnGuiLoad()
        {
            cGuiController.RegisterPane(PaneId, this, GuiPmController.PaneId);
            cGuiController.RegisterButton(PaneId, "back");
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
            cGuiController.DeactivatePane(PaneId);
        }
    }
}
