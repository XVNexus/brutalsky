using Brutalsky;
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
            var previewElement = mapTileCell.Q<VisualElement>("preview");
            var preview = new Painter2D();
            DrawPreviewBackground(preview);
            DrawMapPreview(map, preview);
            var previewVector = ScriptableObject.CreateInstance<VectorImage>();
            preview.SaveToVectorImage(previewVector);
            preview.Dispose();
            previewElement.style.backgroundImage = new StyleBackground(previewVector);

            // Add map tile to map picker view
            mapTileBox.Add(mapTileCell);
            var container = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            container.Add(mapTileBox);
        }

        public void DrawPreviewBackground(Painter2D painter)
        {
            painter.BeginPath();
            painter.MoveTo(new Vector2(-8f, -8f));
            painter.LineTo(new Vector2(208f, -8f));
            painter.LineTo(new Vector2(208f, 108f));
            painter.LineTo(new Vector2(-8f, 108f));
            painter.ClosePath();
            painter.fillColor = new Color(.5f, .5f, .5f, 1/255f);
            painter.Fill();
        }

        public void DrawMapPreview(BsMap map, Painter2D painter)
        {
            foreach (var obj in map.Objects.Values) switch (obj.Tag)
            {
                case Tags.ShapeSym:
                    DrawShapePreview(map, (BsShape)obj, painter);
                    break;
                case Tags.PoolSym:
                    DrawPoolPreview(map, (BsPool)obj, painter);
                    break;
            }
        }

        public void DrawShapePreview(BsMap map, BsShape shape, Painter2D painter)
        {
            DrawObjectPreview(map, shape.Transform, shape.Form.ToPoints(), shape.Color.Tint, painter);
        }

        public void DrawPoolPreview(BsMap map, BsPool pool, Painter2D painter)
        {
            var points = new[]
            {
                new Vector2(-pool.Size.x * .5f, -pool.Size.y * .5f),
                new Vector2(pool.Size.x * .5f, -pool.Size.y * .5f),
                new Vector2(pool.Size.x * .5f, pool.Size.y * .5f),
                new Vector2(-pool.Size.x * .5f, pool.Size.y * .5f)
            };
            DrawObjectPreview(map, pool.Transform, points, pool.Color.Tint, painter);
        }

        public void DrawObjectPreview(BsMap map, ObjectTransform transform, Vector2[] points, Color color,
            Painter2D painter)
        {
            var clamp = map.Size * .5f;
            var translation = transform.Position;
            var rotation = transform.Rotation * Mathf.Deg2Rad;
            painter.BeginPath();
            var transformedStart = TransformDrawPoint(clamp, points[0], translation, rotation);
            var scaledStart = new Vector2(transformedStart.x * 200f / map.Size.x + 100f,
                -transformedStart.y * 100f / map.Size.y + 50f);
            painter.MoveTo(scaledStart);
            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];
                var transformedPoint = TransformDrawPoint(clamp, point, translation, rotation);
                var scaledPoint = new Vector2(transformedPoint.x * 200f / map.Size.x + 100f,
                    -transformedPoint.y * 100f / map.Size.y + 50f);
                painter.LineTo(scaledPoint);
            }
            painter.ClosePath();
            painter.fillColor = color;
            painter.Fill();
        }

        public Vector2 TransformDrawPoint(Vector2 clamp, Vector2 point, Vector2 translation, float rotation)
        {
            var result = MathfExt.TransformVector(point, translation, rotation);
            return new Vector2(Mathf.Clamp(result.x, -clamp.x, clamp.x),
                Mathf.Clamp(result.y, -clamp.y, clamp.y));
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
