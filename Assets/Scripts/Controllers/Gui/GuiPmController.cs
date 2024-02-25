using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controllers.Gui
{
    public class GuiPmController : MonoBehaviour
    {
        // Constants
        public const string PaneId = "pm";

        // References
        private GuiController cGuiController;
        private VisualElement cPaneVisualElement;

        // Events
        private void Start()
        {
            EventSystem.current.OnGuiLoad += OnGuiLoad;

            cGuiController = GetComponent<GuiController>();
        }

        private void OnGuiLoad()
        {
            cPaneVisualElement = cGuiController.GetPane(PaneId);

            var contButton = cGuiController.GetElement<Button>(PaneId, "cont");
            contButton.clicked += OnCont;

            var lvlsButton = cGuiController.GetElement<Button>(PaneId, "lvls");
            lvlsButton.clicked += OnLvls;
            var prevButton = cGuiController.GetElement<Button>(PaneId, "prev");
            prevButton.clicked += OnPrev;
            var restButton = cGuiController.GetElement<Button>(PaneId, "rest");
            restButton.clicked += OnRest;
            var nextButton = cGuiController.GetElement<Button>(PaneId, "next");
            nextButton.clicked += OnNext;

            var cnfgButton = cGuiController.GetElement<Button>(PaneId, "cnfg");
            cnfgButton.clicked += OnCnfg;
            var helpButton = cGuiController.GetElement<Button>(PaneId, "help");
            helpButton.clicked += OnHelp;

            var menuButton = cGuiController.GetElement<Button>(PaneId, "menu");
            menuButton.clicked += OnMenu;
            var exitButton = cGuiController.GetElement<Button>(PaneId, "exit");
            exitButton.clicked += OnExit;
        }

        private static void OnCont()
        {
            Debug.Log("OnCont");
        }

        private static void OnLvls()
        {
            Debug.Log("OnLvls");
        }

        private static void OnPrev()
        {
            Debug.Log("OnPrev");
        }

        private static void OnRest()
        {
            Debug.Log("OnRest");
        }

        private static void OnNext()
        {
            Debug.Log("OnNext");
        }

        private static void OnCnfg()
        {
            Debug.Log("OnCnfg");
        }

        private static void OnHelp()
        {
            Debug.Log("OnHelp");
        }

        private static void OnMenu()
        {
            Debug.Log("OnMenu");
        }

        private static void OnExit()
        {
            Debug.Log("OnExit");
        }
    }
}
