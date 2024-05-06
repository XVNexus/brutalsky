using System.Linq;
using Brutalsky;
using Brutalsky.Object;
using Controllers;
using Controllers.Base;
using Controllers.Player;
using UnityEngine;
using Utils.Map;
using Utils.Player;

namespace Core
{
    public class GameManager : BsBehavior
    {
        // Static instance
        public static GameManager _ { get; private set; }
        private void Awake() => _ = this;

        // Local constants
        public bool RegenerateMaps = false;

        // Local variables
        public bool mapChangeActive;

        // Init functions
        protected override void OnStart()
        {
            EventSystem._.OnPlayerDie += OnPlayerDie;
        }

        protected override void OnLoad()
        {
            if (!RegenerateMaps) return;

            // Generate builtin custom maps
            MapSystem.ResaveBuiltinMaps(new[] { "Brutalsky", "Doomring", "Tossup", "Void" });

            // Generate box maps
            var shapeNames = new[] { "Platform", "Box", "Cage", "Tunnel" };
            var shapes = new[] { 0b1000, 0b1011, 0b1111, 0b1100 };
            var sizeNames = new[] { "Small", "Medium", "Large" };
            var sizes = new[] { new Vector2(20f, 10f), new Vector2(40f, 20f), new Vector2(80f, 40f) };
            for (var i = 0; i < shapes.Length; i++) for (var j = 0; j < sizes.Length; j++)
            {
                MapSystem.SaveMap(MapGenerator.Box($"{sizeNames[j]} {shapeNames[i]}", shapes[i], sizes[j]));
            }

            // TODO: GENERATE PLATFORMER MAPS

            // TODO: GENERATE TERRAIN MAPS

            // TODO: GENERATE MAZE MAPS
        }

        protected override void OnLink()
        {
            InitGame(MapSystem.GenerateId("Void", "Xveon"), new[]
            {
                new BsPlayer(PlayerType.Main, "Player 1", new Color(1f, .5f, 0f)),
                new BsPlayer(PlayerType.Dummy, "Player 2", new Color(0f, .5f, 1f))
            });
        }

        // System functions
        public void InitGame(uint starterMapId, BsPlayer[] activePlayers)
        {
            MapSystem._.LoadAllMapFiles();
            MapSystem._.RegisterPlayers(activePlayers);
            MapSystem._.BuildMap(starterMapId);
            MapSystem._.SpawnAllPlayers();
        }

        public void ReloadData()
        {
            MapSystem._.LoadAllMapFiles();
        }

        public void StartRound(uint mapId)
        {
            CancelInvoke();
            ChangeMap(mapId, true, 1.5f);
        }

        public void RestartRound()
        {
            CancelInvoke();
            ChangeMap(0, false, 1f);
        }

        public void ChangeMap(uint mapId, bool moveCam, float animTime)
        {
            // Fade out view to hide level loading and fade back in with new level
            var camCover = CameraSystem._.cCameraCover.gameObject;
            mapChangeActive = true;
            TimeSystem._.ForceUnpause();
            camCover.LeanColor(new Color(.2f, .2f, .2f), animTime * .4f)
                .setEaseInOutCubic()
                .setOnComplete(() =>
                {
                    MapSystem._.SetAllPlayersFrozen(true);
                    MapSystem._.BuildMap(mapId);
                    MapSystem._.SpawnAllPlayers();
                    camCover.LeanDelayedCall(animTime * .2f, () =>
                        {
                            MapSystem._.SetAllPlayersFrozen(false);
                            camCover.LeanColor(new Color(.2f, .2f, .2f, 0f), animTime * .4f)
                                .setEaseInOutCubic()
                                .setOnComplete(() =>
                                {
                                    mapChangeActive = false;
                                    TimeSystem._.RemoveForcePause();
                                });
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

        // Event functions
        private void OnPlayerDie(BsMap map, BsPlayer player)
        {
            var livingPlayers = (from activePlayer in MapSystem._.ActivePlayers.Values
                where ((PlayerController)activePlayer.InstanceController).GetSub<PlayerHealthController>("health").alive
                select activePlayer).ToList();
            if (livingPlayers.Count != 1) return;
            Debug.Log($"{livingPlayers[0].Name} wins!");
            Invoke(nameof(RestartRound), 3f);
        }
    }
}
