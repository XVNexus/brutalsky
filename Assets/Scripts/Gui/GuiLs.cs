using Brutalsky;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Gui;

namespace Gui
{
    public class GuiLs : BsBehavior
    {
        public const string PaneId = "ls";

        protected override void OnStart()
        {
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPm.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back");

            foreach (var rawMap in MapSystem._.RawMapList.Values)
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
                    var map = BsMap.Parse(MapSystem._.RawMapList[mapId]);
                    MapSystem._.Unbuild();
                    MapSystem._.Build(map);
                    PlayerSystem._.Spawn(map);
                    break;
            }
        }

        private void OnButtonPressBack()
        {
            GuiSystem._.DeactivatePane(PaneId);
        }

        private void AddMapTile(BsMap map)
        {
            var mapTileBox = new VisualElement();
            mapTileBox.AddToClassList("bs");
            mapTileBox.AddToClassList("bs-box");

            var resource = Resources.Load<VisualTreeAsset>($"Gui/Elements/MapTile");
            var mapTileCell = resource.Instantiate();
            mapTileCell.AddToClassList("bs");
            mapTileCell.AddToClassList("bs-cell");
            GuiSystem._.RegisterButton(mapTileCell.Q<Button>("button"), PaneId, $"load-{map.Id}");
            mapTileCell.Q<Label>("title").text = $"<b>{map.Title}</b>\n{map.Author}";
            // mapTileCell.Q<VisualElement>("preview").style.backgroundImage = new StyleBackground(preview);

            mapTileBox.Add(mapTileCell);
            var container = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            container.Add(mapTileBox);
        }
    }
}
