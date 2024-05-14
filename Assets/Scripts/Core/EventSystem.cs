using System;
using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class EventSystem : BsBehavior
    {
        // Static instance
        public static EventSystem _ { get; private set; }
        private void Awake() => _ = this;

        // External references
        public InputActionAsset aInputAction;

        // System functions
        public InputAction GetInputAction(string mapId, string actionId)
        {
            var result = aInputAction.FindActionMap(mapId).FindAction(actionId);
            if (!result.enabled)
            {
                result.Enable();
            }
            return result;
        }

        // Event functions
        public void EmitPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visble)
            => OnPlayerSpawn?.Invoke(map, player, position, visble);
        public event Action<BsMap, BsPlayer, Vector2, bool> OnPlayerSpawn;

        public void EmitPlayerDie(BsMap map, BsPlayer player) => OnPlayerDie?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerDie;

        public void EmitMapPreload(BsMap map) => OnMapPreload?.Invoke(map);
        public event Action<BsMap> OnMapPreload;

        public void EmitMapsUnload() => OnMapsUnload?.Invoke();
        public event Action OnMapsUnload;

        public void EmitMapBuild(BsMap map) => OnMapBuild?.Invoke(map);
        public event Action<BsMap> OnMapBuild;

        public void EmitMapCleanup(BsMap map) => OnMapCleanup?.Invoke(map);
        public event Action<BsMap> OnMapCleanup;

        public void EmitMapUnbuild(BsMap map) => OnMapUnbuild?.Invoke(map);
        public event Action<BsMap> OnMapUnbuild;
    }
}
