using Brutalsky;
using Brutalsky.Map;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;
using Utils.Ext;
using Utils.Object;
using Utils.Shape;

namespace Core
{
    public class GameManager : BsBehavior
    {
        // Static instance
        public static GameManager _ { get; private set; }
        private void Awake() => _ = this;

        // Init functions
        protected override void OnLoad()
        {
            // MapSystem._.ResaveBuiltinMaps(new[] { "Doomring", "Doomring", "Tossup", "Void" });
            // GenerateDefaultMaps();
            StartGame(MapSystem.GenerateId("Void", "Xveon"), new[]
            {
                new BsPlayer("Player 1", 100f, new Color(1f, .5f, 0f)),
                new BsPlayer("Player 2", 100f, new Color(0f, .5f, 1f), true)
            });
        }

        // System functions
        public void StartGame(uint starterMapId, BsPlayer[] activePlayers)
        {
            MapSystem._.ScanMapFiles();
            MapSystem._.RegisterPlayers(activePlayers);
            MapSystem._.BuildMap(starterMapId);
            MapSystem._.SpawnAllPlayers();
        }

        public void StartRound(uint mapId)
        {
            ChangeMap(mapId, true, 2f);
        }

        public void RestartRound()
        {
            ChangeMap(MapSystem._.ActiveMap.Id, false, 2f);
        }

        public void ChangeMap(uint mapId, bool moveCam, float animTime)
        {
            // Disable player physics for the animation
            MapSystem._.SetAllPlayersFrozen(true);
            LeanTween.delayedCall(MapSystem._.gPlayerParent, animTime, () =>
            {
                MapSystem._.SetAllPlayersFrozen(false);
            });

            // Fade out view to hide level loading and fade back in with new level
            var camCover = CameraSystem._.cCameraCover.gameObject;
            camCover.LeanColor(MapSystem._.ActiveMap.BackgroundColor.MultiplyTint(.75f), animTime * .4f)
                .setEaseInOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.BuildMap(mapId);
                    MapSystem._.SpawnAllPlayers();
                    camCover.LeanColor(MapSystem._.ActiveMap.BackgroundColor.MultiplyTint(.75f), animTime * .2f)
                        .setOnComplete(() =>
                        {
                            camCover.LeanColor(MapSystem._.ActiveMap.BackgroundColor.SetAlpha(0f).MultiplyTint(.75f), animTime * .4f)
                                .setEaseInOutCubic();
                        });
                });

            // Move camera down as if the old level ascends into the sky and is replaced by the new level from below
            if (!moveCam) return;
            var camMount = CameraSystem._.gCameraMount;
            camMount.LeanMoveLocal(new Vector2(0f, CameraSystem._.orthoSize * -3f), animTime * .5f)
                .setEaseInQuint()
                .setOnComplete(() =>
                {
                    camMount.transform.localPosition = new Vector2(0f, CameraSystem._.orthoSize * 3f);
                    camMount.LeanMoveLocal(new Vector2(0f, 0f), animTime * .5f)
                        .setEaseOutQuint();
                });
        }

        // TODO: TEMPORARY FUNCTIONS
        private static void GenerateDefaultMaps()
        {
            var shapes = new[] { 0b1000, 0b1011, 0b1111, 0b1100 };
            var shapeNames = new[] { "Platform", "Box", "Cage", "Tunnel" };
            var sizes = new[] { 20f, 40f, 80f };
            var sizeNames = new[] { "Small", "Medium", "Large" };
            for (var i = 0; i < shapes.Length; i++) for (var j = 0; j < sizes.Length; j++)
            {
                GenerateBoxMap($"{sizeNames[j]} {shapeNames[i]}", shapes[i], sizes[j]);
            }
        }

        private static void GenerateBoxMap(string title, int shape, float size)
        {
            var bottom = (shape & 0b1000) > 0;
            var top = (shape & 0b0100) > 0;
            var left = (shape & 0b0010) > 0;
            var right = (shape & 0b0001) > 0;
            var map = new BsMap(title, "Brutalsky")
            {
                PlayArea = new Vector2(size, size * .5f),
                BackgroundColor = new Color(.025f, .25f, .025f),
                LightingColor = new Color(.1f, 1f, .1f, .8f),
                GravityDirection = Direction.Down,
                GravityStrength = 20f,
                PlayerHealth = 100f
            };
            map.AddSpawn(new BsSpawn(new Vector2(-3f, -size * .25f + 1.5f), 0));
            map.AddSpawn(new BsSpawn(new Vector2(-1f, -size * .25f + 1.5f), 0));
            map.AddSpawn(new BsSpawn(new Vector2(1f, -size * .25f + 1.5f), 0));
            map.AddSpawn(new BsSpawn(new Vector2(3f, -size * .25f + 1.5f), 0));
            if (bottom) map.AddObject(new BsShape
            (
                "wall-bottom",
                new ObjectTransform(0f, -size * .25f + .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size, 1f)
            ));
            if (top) map.AddObject(new BsShape
            (
                "wall-top",
                new ObjectTransform(0f, size * .25f - .5f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(size, 1f)
            ));
            if (left) map.AddObject(new BsShape
            (
                "wall-left",
                new ObjectTransform(-size * .5f + .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size * .5f)
            ));
            if (right) map.AddObject(new BsShape
            (
                "wall-right",
                new ObjectTransform(size * .5f - .5f, 0f),
                ObjectLayer.Midground,
                true,
                Form.Rectangle(1f, size * .5f)
            ));
            if (top && left) map.AddObject(new BsShape
            (
                "corner-tl",
                new ObjectTransform(-size * .5f + 1f, size * .25f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, -3f })
            ));
            if (top && right) map.AddObject(new BsShape
            (
                "corner-tr",
                new ObjectTransform(size * .5f - 1f, size * .25f - 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, -3f })
            ));
            if (bottom && left) map.AddObject(new BsShape
            (
                "corner-bl",
                new ObjectTransform(-size * .5f + 1f, -size * .25f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, 3f, 0f, 1f, 0f, 0f, 0f, 3f })
            ));
            if (bottom && right) map.AddObject(new BsShape
            (
                "corner-br",
                new ObjectTransform(size * .5f - 1f, -size * .25f + 1f),
                ObjectLayer.Midground,
                true,
                Form.Vector(new[] { 0f, 0f, 0f, -3f, 0f, 1f, 0f, 0f, 0f, 3f })
            ));
            MapSystem.SaveMap(map);
        }
    }
}
