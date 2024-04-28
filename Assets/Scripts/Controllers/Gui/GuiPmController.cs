using System;
using Controllers.Base;
using Core;
using UnityEngine;
using Utils.Gui;

namespace Controllers.Gui
{
    public class GuiPmController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "pm";

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this);
            GuiSystem._.RegisterButtons(PaneId, new[]
            {
                "cont", "lvls", "rest", "cnfg", "help", "menu", "exit"
            });
        }

        // Event functions
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
                case "rest":
                    OnButtonPressRest();
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
            GuiSystem._.Escape();
        }

        private void OnButtonPressLvls()
        {
            GuiSystem._.ActivatePane(GuiLsController.PaneId);
        }

        private void OnButtonPressRest()
        {
            GameManager._.RestartRound();
        }

        private void OnButtonPressCnfg()
        {
            GuiSystem._.ActivatePane(GuiCfController.PaneId);
        }

        private void OnButtonPressHelp()
        {
            GuiSystem._.ActivatePane(GuiHeController.PaneId);
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
