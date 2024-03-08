using Brutalsky;
using Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controllers.Gui
{
    public class GuiLsController : MonoBehaviour
    {
        // Constants
        public const string PaneId = "ls";

        // References
        private GuiController cGuiController;

        // Functions
        private void AddMapTile(BsMap map)
        {
            var mapTileBox = new VisualElement();
            mapTileBox.AddToClassList("bs");
            mapTileBox.AddToClassList("bs-box");

            var resource = Resources.Load<VisualTreeAsset>($"Gui/Elements/MapTile");
            var mapTileCell = resource.Instantiate();
            mapTileCell.AddToClassList("bs");
            mapTileCell.AddToClassList("bs-cell");
            cGuiController.RegisterButton(mapTileCell.Q<Button>("button"), PaneId, $"load-{map.id}");
            mapTileCell.Q<Label>("title").text = $"<b>{map.title}</b>\n{map.author}";
            // mapTileCell.Q<VisualElement>("preview").style.backgroundImage = new StyleBackground(preview);

            mapTileBox.Add(mapTileCell);
            var container = cGuiController.GetPane(PaneId).element.Q<VisualElement>("unity-content-container");
            container.Add(mapTileBox);
        }

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

            foreach (var rawMap in MapSystem.current.rawMapList.Values)
            {
                AddMapTile(BsMap.Parse(rawMap));
            }
        }

        private void OnGuiAction(GuiAction action, string paneId, string itemId)
        {
            if (paneId != PaneId) return;
            switch (itemId[..4])
            {
                case "back":
                    OnButtonPressBack();
                    break;
                case "load":
                    var mapId = uint.Parse(itemId[5..]);
                    var map = BsMap.Parse(MapSystem.current.rawMapList[mapId]);
                    MapSystem.current.Unbuild();
                    MapSystem.current.Build(map);
                    PlayerSystem.current.Spawn(map);
                    break;
            }
        }

        private void OnButtonPressBack()
        {
            cGuiController.DeactivatePane(PaneId);
        }
    }
}
