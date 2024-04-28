using System.Collections.Generic;
using System.Linq;
using Brutalsky;
using Brutalsky.Base;
using Controllers.Base;
using Core;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Constants;
using Utils.Ext;
using Utils.Gui;
using Utils.Object;

namespace Controllers.Gui
{
    public class GuiLsController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "ls";

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnGuiAction += OnGuiAction;
        }

        protected override void OnLoad()
        {
            GuiSystem._.RegisterPane(PaneId, this, GuiPmController.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back");

            foreach (var rawMap in MapSystem._.RawMapList.Values)
            {
                AddMapTile(BsMap.Parse(rawMap));
            }
        }

        // Gui functions
        private void AddMapTile(BsMap map)
        {
            // Create new map tile element
            var mapTileBox = new VisualElement();
            mapTileBox.AddToClassList("bs");
            mapTileBox.AddToClassList("bs-box");
            var mapTileCell = Resources.Load<VisualTreeAsset>("Gui/Elements/MapTile").Instantiate();
            mapTileCell.AddToClassList("bs");
            mapTileCell.AddToClassList("bs-cell");

            // Register load map button
            GuiSystem._.RegisterButton(mapTileCell.Q<Button>("button"), PaneId, $"load-{map.Id}");
            mapTileCell.Q<Label>("title").text = $"<b>{map.Title}</b>\n{map.Author}";

            // Render map preview
            var preview = new GuiPainter(new Rect(map.PlayArea * -.5f, map.PlayArea), new Vector2(200f, 100f));
            PaintMapPreview(preview, map);
            mapTileCell.Q<VisualElement>("preview").style.backgroundImage = new StyleBackground(preview.Print());

            // Add map tile to map picker view
            mapTileBox.Add(mapTileCell);
            var container = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            container.Add(mapTileBox);
        }

        public void PaintMapPreview(GuiPainter painter, BsMap map)
        {
            // Paint background and frame
            painter.AutoTransform = false;
            painter.DrawRoundedRect(new Rect(-3f, -3f, 206f, 106f), 3f);
            painter.Fill(map.BackgroundColor.Tint * map.LightingColor.Tint);
            painter.DrawRoundedRect(new Rect(-2f, -2f, 204f, 104f), 2f);
            painter.Stroke(new Color(1f, 1f, 1f, .25f), 2f);
            painter.AutoTransform = true;

            // Sort objects by layer
            var objects = new Dictionary<ObjectLayer, List<BsObject>>
            {
                [ObjectLayer.Background] = new(),
                [ObjectLayer.Midground] = new(),
                [ObjectLayer.Foreground] = new()
            };
            foreach (var obj in map.Objects.Values)
            {
                objects[obj.Layer].Add(obj);
            }

            // Paint objects
            foreach (var obj in objects.Keys.SelectMany(layer => objects[layer])) switch (obj.Tag)
            {
                case Tags.ShapeSym:
                    var shape = (BsShape)obj;
                    PaintObjectPreview(painter, shape.Transform, shape.Form.ToFillPoints(shape.Transform.Rotation),
                        shape.Color.Color * map.LightingColor.Tint);
                    break;
                case Tags.PoolSym:
                    var pool = (BsPool)obj;
                    var rotation = pool.Transform.Rotation;
                    var points = new[]
                    {
                        MathfExt.RotateVector(new Vector2(-pool.Size.x * .5f, -pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(pool.Size.x * .5f, -pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(pool.Size.x * .5f, pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(-pool.Size.x * .5f, pool.Size.y * .5f), rotation)
                    };
                    PaintObjectPreview(painter, pool.Transform, points, pool.Color.Color * map.LightingColor.Tint);
                    break;
            }

            // Paint spawns
            foreach (var spawn in map.Spawns)
            {
                PaintSpawnPreview(painter, spawn.Position);
            }
        }

        public void PaintSpawnPreview(GuiPainter painter, Vector2 position)
        {
            painter.DrawCircle(position, .5f);
            painter.Fill(new Color(1f, 1f, 1f));
            painter.DrawCircle(position, 1f);
            painter.Stroke(new Color(1f, 1f, 1f, .1f), 2f);
        }

        public void PaintObjectPreview(GuiPainter painter, ObjectTransform transform, Vector2[] points, Color color)
        {
            var transformedPoints = new Vector2[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                transformedPoints[i] = MathfExt.TranslateVector(points[i], transform.Position);
            }
            painter.DrawPolygon(transformedPoints);
            painter.Fill(color);
        }

        // Event functions
        private void OnGuiAction(GuiAction action, string paneId, string itemId)
        {
            if (paneId != PaneId) return;
            switch (itemId[..4])
            {
                case "back":
                    OnButtonPressBack();
                    break;
                case "load":
                    OnButtonPressLoad(uint.Parse(itemId[5..]));
                    break;
            }
        }

        private void OnButtonPressBack()
        {
            GuiSystem._.DeactivatePane(PaneId);
        }

        private void OnButtonPressLoad(uint mapId)
        {
            MapSystem._.Unbuild();
            MapSystem._.Build(mapId);
            PlayerSystem._.SpawnAll(MapSystem._.ActiveMap);
        }
    }
}
