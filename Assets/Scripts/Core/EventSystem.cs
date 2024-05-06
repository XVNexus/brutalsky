using System;
using Brutalsky;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine.InputSystem;
using Utils.Gui;

namespace Core
{
    public class EventSystem : BsBehavior
    {
        // Static instance
        public static EventSystem _ { get; private set; }
        private void Awake() => _ = this;

        // External references
        public InputActionAsset aInputAction;

        // Event functions
        public void EmitMapPreload(BsMap map) => OnMapPreload?.Invoke(map);
        public event Action<BsMap> OnMapPreload;

        public void EmitMapBuild(BsMap map) => OnMapBuild?.Invoke(map);
        public event Action<BsMap> OnMapBuild;

        public void EmitMapUnbuild(BsMap map) => OnMapUnbuild?.Invoke(map);
        public event Action<BsMap> OnMapUnbuild;

        public void EmitPlayerSpawn(BsMap map, BsPlayer player) => OnPlayerSpawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerSpawn;

        public void EmitPlayerDie(BsMap map, BsPlayer player) => OnPlayerDie?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerDie;
    }
}
