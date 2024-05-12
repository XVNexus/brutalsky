using System.Linq;
using Brutalsky;
using Brutalsky.Object;
using Controllers;
using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils.Ext;
using Utils.Map;
using Utils.Player;

namespace Core
{
    public class GameManager : BsBehavior
    {
        // Static instance
        public static GameManager _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public bool autoRestart;
        public Color loadingColor;

        // Local variables
        private bool _mapChangeActive;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnPlayerDie += OnPlayerDie;
        }

        private void OnDestroy()
        {
            EventSystem._.OnPlayerDie -= OnPlayerDie;
        }

        protected override void OnLink()
        {
            LoadData();
            InitMap(MapSystem.GenerateId("Void", "Xveon"), new[]
            {
                new BsPlayer(PlayerType.Main, "Player 1", new Color(1f, .5f, 0f)),
                new BsPlayer(PlayerType.Dummy, "Player 2", new Color(0f, .5f, 1f))
            }, 1f);
        }

        // System functions
        public static void LoadData()
        {
            // Clear any previously loaded maps
            if (MapSystem._.MapList.Count > 0)
            {
                MapSystem._.UnregisterMaps();
            }

            // Load builtin maps
            MapSystem._.RegisterMaps(ResourceSystem._.LoadMapAssets(new[]
            {
                "Void"/*, "Brutalsky", "Doomring", "Tossup", "Racetrack"*/
            }));

            // Load custom maps
            MapSystem._.RegisterMaps(ResourceSystem._.LoadMapFiles());

            // Generate box maps
            var shapes = new[] { 0b1000, 0b1011, 0b1111, 0b1100 };
            var sizes = new[] { new Vector2(20f, 10f), new Vector2(40f, 20f), new Vector2(80f, 40f) };
            for (var i = 0; i < shapes.Length; i++) for (var j = 0; j < sizes.Length; j++)
            {
                MapSystem._.RegisterMap(MapGenerator.Box($"Box {i * sizes.Length + j + 1}", shapes[i], sizes[j]));
            }

            // Generate platformer maps
            const int variantCount = 5;
            for (var i = 1; i < variantCount + 1; i++)
            {
                MapSystem._.RegisterMap(MapGenerator.Platformer($"Platformer {i}", 50f + i * 50f,
                    (uint)(i * 0x69), i < variantCount ? $"Platformer {i + 1}" : ""));
            }

            // TODO: GENERATE TERRAIN MAPS

            // TODO: GENERATE MAZE MAPS
        }

        public bool StartRound(uint mapId)
        {
            return mapId != (MapSystem._.ActiveMap?.Id ?? 0) ? ChangeMap(mapId, true, 1.5f) : RestartRound();
        }

        public bool RestartRound()
        {
            return ChangeMap(0, false, 1f);
        }

        public void InitMap(uint starterMapId, BsPlayer[] activePlayers, float animationTime)
        {
            MapSystem._.RegisterPlayers(activePlayers);
            MapSystem._.BuildMap(starterMapId);
            MapSystem._.SpawnPlayers();
            CameraSystem._.cCameraCover.gameObject.LeanColor(loadingColor.SetAlpha(0f), animationTime * .4f)
                .setEaseInOutCubic();
        }

        public bool ChangeMap(uint mapId, bool moveCam, float animationTime)
        {
            if (_mapChangeActive) return false;
            CancelInvoke();

            // Fade out view to hide level loading and fade back in with new level
            var camCover = CameraSystem._.cCameraCover.gameObject;
            _mapChangeActive = true;
            TimeSystem._.ForceUnpause();
            camCover.LeanColor(loadingColor, animationTime * .4f)
                .setEaseInOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.BuildMap(mapId);
                    MapSystem._.SpawnPlayers();
                    camCover.LeanDelayedCall(animationTime * .2f, () =>
                        {
                            camCover.LeanColor(loadingColor.SetAlpha(0f), animationTime * .4f)
                                .setEaseInOutCubic()
                                .setOnComplete(() =>
                                {
                                    _mapChangeActive = false;
                                    TimeSystem._.RemoveForcePause();
                                });
                        });
                });

            // Move camera down as if the old level ascends into the sky and is replaced by the new level from below
            if (!moveCam) return true;
            var camMount = CameraSystem._.gCameraMount;
            camMount.LeanMoveLocal(new Vector2(0f, CameraSystem._.ViewSize * -3f), animationTime * .5f)
                .setEaseInQuint()
                .setOnComplete(() =>
                {
                    camMount.transform.localPosition = new Vector2(0f, CameraSystem._.ViewSize * 3f);
                    camMount.LeanMoveLocal(new Vector2(0f, 0f), animationTime * .5f)
                        .setEaseOutQuint();
                });

            return true;
        }

        // Event functions
        private void OnPlayerDie(BsMap map, BsPlayer player)
        {
            if (_mapChangeActive) return;
            var livingPlayers = (from activePlayer in MapSystem._.ActivePlayers.Values
                where ((PlayerController)activePlayer.InstanceController).GetSub<PlayerHealthController>("health").Alive
                    && !MapSystem._.GetPlayerFrozen(player.InstanceObject)
                select activePlayer).ToList();
            if ((autoRestart && livingPlayers.Count == 1) || livingPlayers.Count == 0)
            {
                Invoke(nameof(RestartRound), 3f);
            }
        }
    }
}
