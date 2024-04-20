using System;
using Brutalsky;
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
        public void EmitGuiAction(GuiAction action, string paneId, string itemId) => OnGuiAction?.Invoke(action, paneId, itemId);
        public event Action<GuiAction, string, string> OnGuiAction;

        public void EmitMapBuild(BsMap map) => OnMapBuild?.Invoke(map);
        public event Action<BsMap> OnMapBuild;

        public void EmitMapUnbuild(BsMap map) => OnMapUnbuild?.Invoke(map);
        public event Action<BsMap> OnMapUnbuild;

        public void EmitPlayerSpawn(BsMap map, BsPlayer player) => OnPlayerSpawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerSpawn;

        public void EmitPlayerDespawn(BsMap map, BsPlayer player) => OnPlayerDespawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerDespawn;

        public void EmitPlayerRespawn(BsMap map, BsPlayer player) => OnPlayerRespawn?.Invoke(map, player);
        public event Action<BsMap, BsPlayer> OnPlayerRespawn;
    }
}
