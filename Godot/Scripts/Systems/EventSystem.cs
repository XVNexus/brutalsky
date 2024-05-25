using System;
using Brutalsky.Scripts.Config;
using Brutalsky.Scripts.Data;
using Godot;

namespace Brutalsky.Scripts.Systems;

public partial class EventSystem : Node
{
	// Singleton instance
	public static EventSystem Inst { get; private set; } = null!;
	public override void _EnterTree() => Inst = this;

	// Exposed properties
	public event Action<ConfigList> OnConfigUpdate;
	public event Action<BsPlayer> OnPlayerRegister;
	public event Action<BsPlayer> OnPlayerUnregister;
	public event Action<BsMap, BsPlayer, Vector2, bool> OnPlayerSpawn;
	public event Action<BsMap, BsPlayer> OnPlayerDie;
	public event Action<BsMap> OnMapPreload;
	public event Action OnMapsUnload;
	public event Action<BsMap> OnMapBuild;
	public event Action<BsMap> OnMapCleanup;
	public event Action<BsMap> OnMapUnbuild;

	// Event functions
	public void EmitConfigUpdate(ConfigList cfg) => OnConfigUpdate?.Invoke(cfg);

	public void EmitPlayerRegister(BsPlayer player) => OnPlayerRegister?.Invoke(player);

	public void EmitPlayerUnregister(BsPlayer player) => OnPlayerUnregister?.Invoke(player);

	public void EmitPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visble)
		=> OnPlayerSpawn?.Invoke(map, player, position, visble);

	public void EmitPlayerDie(BsMap map, BsPlayer player) => OnPlayerDie?.Invoke(map, player);

	public void EmitMapPreload(BsMap map) => OnMapPreload?.Invoke(map);

	public void EmitMapsUnload() => OnMapsUnload?.Invoke();

	public void EmitMapBuild(BsMap map) => OnMapBuild?.Invoke(map);

	public void EmitMapCleanup(BsMap map) => OnMapCleanup?.Invoke(map);

	public void EmitMapUnbuild(BsMap map) => OnMapUnbuild?.Invoke(map);
}
