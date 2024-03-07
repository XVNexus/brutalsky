using Core;
using UnityEngine;

namespace Controllers.Gui
{
    public class GuiLsController : MonoBehaviour
    {
        // Constants
        public const string PaneId = "ls";

        // References
        private GuiController cGuiController;

        // Events
        private void Start()
        {
            EventSystem.current.OnGuiLoad += OnGuiLoad;
            EventSystem.current.OnGuiAction += OnGuiAction;

            cGuiController = GetComponent<GuiController>();

            cGuiController.RegisterPane(PaneId, this);
        }

        private void OnGuiLoad()
        {
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