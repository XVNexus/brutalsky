using System.Collections.Generic;
using System.Linq;
using Controllers.Base;
using Data;
using Data.Base;
using Data.Object;
using Systems;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Constants;
using Utils.Ext;
using Utils.Gui;

namespace Controllers.Gui
{
    public class GuiLsController : BsBehavior
    {
        // Gui metadata
        public const string PaneId = "ls";

        // Local variables
        private readonly List<string> _loadMapButtons = new();

        // External references
        private VisualTreeAsset _eMapTile;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnMapPreload += OnMapPreload;
            EventSystem._.OnMapsUnload += OnMapsUnload;
        }

        private void OnDestroy()
        {
            EventSystem._.OnMapPreload -= OnMapPreload;
            EventSystem._.OnMapsUnload -= OnMapsUnload;
        }

        protected override void OnLoad()
        {
            _eMapTile = Resources.Load<VisualTreeAsset>("Gui/Elements/MapTile");

            GuiSystem._.RegisterPane(PaneId, this, GuiPmController.PaneId);
            GuiSystem._.RegisterButton(PaneId, "back", () =>
            {
                GuiSystem._.DeactivatePane(PaneId);
            });
            GuiSystem._.RegisterButton(PaneId, "rest", () =>
            {
                if (GameManager._.RestartRound())
                {
                    GuiSystem._.EscapeAll();
                }
            });
            GuiSystem._.RegisterButton(PaneId, "reld", () =>
            {
                GameManager._.LoadMaps();
            });
        }

        // Module functions
        private void AddMapTile(BsMap map)
        {
            // Create new map tile element
            var mapTileBox = new VisualElement();
            mapTileBox.AddToClassList("bs");
            mapTileBox.AddToClassList("bs-box");
            var mapTileCell = _eMapTile.Instantiate();
            mapTileBox.Add(mapTileCell);

            // Register load map button
            mapTileCell.Q<Label>("title").text = $"<b>{map.Title}</b>\n{map.Author}";
            var itemId = $"load-{map.Id}";
            GuiSystem._.RegisterButton(mapTileCell.Q<Button>("button"), PaneId, itemId, () =>
            {
                if (GameManager._.StartRound(map.Id))
                {
                    GuiSystem._.EscapeAll();
                }
            });
            _loadMapButtons.Add(itemId);

            // Render map preview
            var preview = new GuiPainter(map.PlayArea, new Vector2(200f, 100f));
            PaintMapPreview(preview, map);
            mapTileCell.Q<VisualElement>("preview").style.backgroundImage = new StyleBackground(preview.Print());

            // Add map tile to map picker view
            var mapTileContainer = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            mapTileContainer.Add(mapTileBox);
        }

        private void ClearMapTiles()
        {
            // Unregister all load map buttons
            foreach (var itemId in _loadMapButtons)
            {
                GuiSystem._.UnregisterButton(PaneId, itemId);
            }

            // Remove the buttons from the ui
            var mapTileContainer = GuiSystem._.GetPane(PaneId).Element.Q<VisualElement>("unity-content-container");
            mapTileContainer.Clear();
        }

        private void PaintMapPreview(GuiPainter painter, BsMap map)
        {
            // Paint background and frame
            painter.AutoTransform = false;
            painter.DrawRect(new Rect(-3f, -3f, 206f, 106f), 3f);
            painter.Fill(map.BackgroundColor);
            painter.DrawRect(new Rect(-2f, -2f, 204f, 104f), 2f);
            painter.Stroke(new Color(1f, 1f, 1f, .25f), 2f);
            painter.AutoTransform = true;

            // Collect objects by layer
            var objects = new Dictionary<int, List<BsObject>>();
            for (int i = sbyte.MinValue; i <= sbyte.MaxValue; i++)
            {
                objects[i] = new List<BsObject>();
            }
            foreach (var obj in map.Objects.Values) switch (obj.Tag)
            {
                case Tags.ShapePrefix:
                    var shape = (BsShape)obj;
                    objects[shape.Layer].Add(shape);
                    break;
                case Tags.PoolPrefix:
                    var pool = (BsPool)obj;
                    objects[pool.Layer].Add(pool);
                    break;
                case Tags.DecalPrefix:
                    var decal = (BsDecal)obj;
                    objects[decal.Layer].Add(decal);
                    break;
            }

            // Paint objects
            foreach (var obj in objects.Keys.SelectMany(layer => objects[layer])) switch (obj.Tag)
            {
                case Tags.ShapePrefix:
                    var shape = (BsShape)obj;
                    PaintObjectPreview(painter, shape.Position, shape.Path.ToPoints(shape.Rotation),
                        shape.Color * map.LightingTint);
                    break;
                case Tags.PoolPrefix:
                    var pool = (BsPool)obj;
                    var rotation = pool.Rotation;
                    var points = new[]
                    {
                        MathfExt.RotateVector(new Vector2(-pool.Size.x * .5f, -pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(pool.Size.x * .5f, -pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(pool.Size.x * .5f, pool.Size.y * .5f), rotation),
                        MathfExt.RotateVector(new Vector2(-pool.Size.x * .5f, pool.Size.y * .5f), rotation)
                    };
                    PaintObjectPreview(painter, pool.Position, points, pool.Color * map.LightingTint);
                    break;
                case Tags.DecalPrefix:
                    var decal = (BsDecal)obj;
                    PaintObjectPreview(painter, decal.Position, decal.Path.ToPoints(decal.Rotation),
                        decal.Color * map.LightingTint);
                    break;
            }

            // Paint spawns
            foreach (var spawn in map.Spawns)
            {
                PaintSpawnPreview(painter, spawn.Position);
            }
        }

        private void PaintSpawnPreview(GuiPainter painter, Vector2 position)
        {
            painter.DrawCircle(position, .5f);
            painter.Fill(new Color(1f, 1f, 1f));
            painter.DrawCircle(position, 1f);
            painter.Stroke(new Color(1f, 1f, 1f, .1f), 2f);
        }

        private void PaintObjectPreview(GuiPainter painter, Vector2 position, Vector2[] points, Color color)
        {
            var transformedPoints = new Vector2[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                transformedPoints[i] = MathfExt.TranslateVector(points[i], position);
            }
            painter.DrawPolygon(transformedPoints);
            painter.Fill(color);
        }

        // Event functions
        private void OnMapPreload(BsMap map)
        {
            AddMapTile(map);
        }

        private void OnMapsUnload()
        {
            ClearMapTiles();
        }
    }
}
