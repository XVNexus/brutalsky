using System;
using Controllers.Base;
using Core;
using UnityEngine;

namespace Controllers.Gui
{
    public class GuiPmController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "pm";

        // Init functions
        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this);
            GuiSystem._.RegisterButton(PaneId, "cont", () =>
            {
                GuiSystem._.EscapeOne();
            });
            GuiSystem._.RegisterButton(PaneId, "lvls", () =>
            {
                GuiSystem._.ActivatePane(GuiLsController.PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "rest", () =>
            {
                if (GameManager._.RestartRound())
                {
                    GuiSystem._.EscapeAll();
                }
            });
            GuiSystem._.RegisterButton(PaneId, "cnfg", () =>
            {
                GuiSystem._.ActivatePane(GuiCfController.PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "help", () =>
            {
                GuiSystem._.ActivatePane(GuiHeController.PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "menu", () =>
            {
                throw new NotImplementedException();
            });
            GuiSystem._.RegisterButton(PaneId, "exit", () =>
            {
                Application.Quit();
            });
        }
    }
}
