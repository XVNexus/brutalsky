using System;
using Core;
using UnityEngine;
using Utils.Gui;

namespace Gui
{
    public class GuiPm : BsBehavior
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
            GuiSystem._.Escape();
        }

        private void OnButtonPressLvls()
        {
            GuiSystem._.ActivatePane(GuiLs.PaneId);
        }

        private void OnButtonPressPrev()
        {
            OnButtonPressRest();
        }

        private void OnButtonPressRest()
        {
            MapSystem._.Rebuild();
            PlayerSystem._.Spawn(MapSystem._.ActiveMap);
        }

        private void OnButtonPressNext()
        {
            OnButtonPressRest();
        }

        private void OnButtonPressCnfg()
        {
            GuiSystem._.ActivatePane(GuiCf.PaneId);
        }

        private void OnButtonPressHelp()
        {
            GuiSystem._.ActivatePane(GuiHe.PaneId);
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
