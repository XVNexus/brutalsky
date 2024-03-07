using System;
using Core;
using UnityEngine;

namespace Controllers.Gui
{
    public class GuiPmController : MonoBehaviour
    {
        // Constants
        public const string PaneId = "pm";

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
            cGuiController.RegisterButtons(PaneId, new[]
            {
                "cont", "lvls", "prev", "rest", "next", "cnfg", "help", "menu", "exit"
            });
        }

        private void OnGuiAction(GuiAction action, string paneId, string itemId)
        {
            if (paneId != PaneId) return;
            switch (itemId)
            {
                case "cont":
                    OnButtonPressCont();
                    break;
                case "lvls":
                    OnButtonPressLvls();
                    break;
                case "prev":
                    OnButtonPressPrev();
                    break;
                case "rest":
                    OnButtonPressRest();
                    break;
                case "next":
                    OnButtonPressNext();
                    break;
                case "cnfg":
                    OnButtonPressCnfg();
                    break;
                case "help":
                    OnButtonPressHelp();
                    break;
                case "menu":
                    OnButtonPressMenu();
                    break;
                case "exit":
                    OnButtonPressExit();
                    break;
            }
        }

        private void OnButtonPressCont()
        {
            cGuiController.Escape();
        }

        private void OnButtonPressLvls()
        {
            cGuiController.ActivatePane(GuiLsController.PaneId);
        }

        private void OnButtonPressPrev()
        {
            OnButtonPressRest();
        }

        private void OnButtonPressRest()
        {
            MapSystem.current.Rebuild();
            PlayerSystem.current.Spawn(MapSystem.current.activeMap);
        }

        private void OnButtonPressNext()
        {
            OnButtonPressRest();
        }

        private void OnButtonPressCnfg()
        {
            // TODO: DO NOT HARDCODE THIS ID
            cGuiController.ActivatePane("cf");
        }

        private void OnButtonPressHelp()
        {
            // TODO: DO NOT HARDCODE THIS ID
            cGuiController.ActivatePane("he");
        }

        private void OnButtonPressMenu()
        {
            throw new NotImplementedException();
        }

        private void OnButtonPressExit()
        {
            Application.Quit();
        }
    }
}
