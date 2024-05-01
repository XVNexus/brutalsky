using Brutalsky;
using Brutalsky.Map;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;
using Utils.Object;
using Utils.Shape;

namespace Core
{
    public class GameManager : BsBehavior
    {
        // Static instance
        public static GameManager _ { get; private set; }
        private void Awake()
        {
            _ = this;
            GenerateDefaultMaps();
        }

        // Init functions
        protected override void OnLoad()
        {
            StartGame(new[] { "Brutalsky", "Doomring" },
                MapSystem.GenerateId("Brutalsky", "Xveon"), new[]
                {
                    new BsPlayer("Player 1", 100f, new ObjectColor(1f, .5f, 0f)),
                    new BsPlayer("Player 2", 100f, new ObjectColor(0f, .5f, 1f), true)
                });
        }

        // System functions
        public void StartGame(string[] builtinMapFilenames, uint starterMapId, BsPlayer[] activePlayers)
        {
            MapSystem._.ScanMapFiles(builtinMapFilenames);
            MapSystem._.RegisterPlayers(activePlayers);
            MapSystem._.BuildMap(starterMapId);
            MapSystem._.SpawnAllPlayers();
        }

        public void RestartRound()
        {
            MapSystem._.BuildMap();
            MapSystem._.SpawnAllPlayers();
        }

        public void ChangeMap(uint mapId)
        {
            MapSystem._.BuildMap(mapId);
            MapSystem._.SpawnAllPlayers();
        }

        // Utility functions
        // TODO: THIS FUNCTION IS DOGSHIT
        private void GenerateDefaultMaps()
        {
            var sizes = new[] { 20f, 40f, 80f };
            var names = new[] { "Small", "Medium", "Large" };
            for (var i = 0; i < sizes.Length; i++)
            {
                GenerateBoxMap("Platform", sizes[i], names[i], true, false, false, false);
                GenerateBoxMap("Box", sizes[i], names[i], true, false, true, true);
                GenerateBoxMap("Cage", sizes[i], names[i], true, true, true, true);
                GenerateBoxMap("Tunnel", sizes[i], names[i], true, true, false, false);
            }
        }

        // TODO: THIS ONE IS TOO
        private void GenerateBoxMap(string title, float size, string name, bool bottom, bool top, bool left, bool right)
        {
            var baseColor = title switch
            {
                "Platform" => new ObjectColor(1f, .1f, .1f),
                "Box" => new ObjectColor(.1f, 1f, .1f),
                "Cage" => new ObjectColor(.1f, 1f, 1f),
                "Tunnel" => new ObjectColor(1f, 1f, .1f),
                _ => ObjectColor.Ether()
            };
            var map = new BsMap($"{name} {title}", "Brutalsky")
            {
                PlayArea = new Vector2(size, size * .5f),
                BackgroundColor = new ObjectColor(baseColor.Color.r, baseColor.Color.g, baseColor.Color.b, .25f),
                LightingColor = new ObjectColor(baseColor.Color.r, baseColor.Color.g, baseColor.Color.b, .9f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f
            };
            if (bottom) map.AddObject(new BsShape
            (
                "wall-bottom",
                new ObjectTransform(0f, -size * .25f + .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size, 1f),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (top) map.AddObject(new BsShape
            (
                "wall-top",
                new ObjectTransform(0f, size * .25f - .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size, 1f),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (left) map.AddObject(new BsShape
            (
                "wall-left",
                new ObjectTransform(-size * .5f + .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size * .5f),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (right) map.AddObject(new BsShape
            (
                "wall-right",
                new ObjectTransform(size * .5f - .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size * .5f),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (top && left) map.AddObject(new BsShape
            (
                "corner-tl",
                new ObjectTransform(-size * .5f + 1f, size * .25f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f }),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (top && right) map.AddObject(new BsShape
            (
                "corner-tr",
                new ObjectTransform(size * .5f - 1f, size * .25f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f }),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (bottom && left) map.AddObject(new BsShape
            (
                "corner-bl",
                new ObjectTransform(-size * .5f + 1f, -size * .25f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f }),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            if (bottom && right) map.AddObject(new BsShape
            (
                "corner-br",
                new ObjectTransform(size * .5f - 1f, -size * .25f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f }),
                ShapeMaterial.Stone(),
                ObjectColor.Ether()
            ));
            map.AddSpawn(new BsSpawn(new Vector2(-1f, -size * .25f + 1.5f)));
            map.AddSpawn(new BsSpawn(new Vector2(1f, -size * .25f + 1.5f)));
            MapSystem.SaveMap(map);
        }
    }
}