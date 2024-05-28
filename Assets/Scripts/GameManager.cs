using System.Collections.Generic;
using System.Linq;
using Config;
using Controllers.Base;
using Data;
using Extensions;
using Maps;
using Systems;
using UnityEngine;

public class GameManager : BsBehavior
{
    // Static instance
    public static GameManager _ { get; private set; }
    private void Awake() => _ = this;

    // Config options
    public float transitionTime;
    public Color loadingColor;
    private uint _cfgStartingMap;
    private bool _cfgAutoRestart;
    private bool _cfgUsePlayer1;
    private bool _cfgUsePlayer2;
    private bool _cfgEnableCustomMaps;
    private bool _cfgEnableBoxMaps;
    private bool _cfgEnablePlatformerMaps;
    private bool _cfgEnableTerrainMaps;
    private bool _cfgEnableMazeMaps;

    // Local variables
    private BsPlayer _player1 = new("Player 1") { Type = BsPlayer.TypeLocal1, Color = new Color(1f, .5f, 0f) };
    private BsPlayer _player2 = new("Player 2") { Type = BsPlayer.TypeLocal2, Color = new Color(0f, .5f, 1f) };
    private Dictionary<string, BsPlayer> _livingPlayers = new();
    private bool _mapChangeActive;

    // Init functions
    protected override void OnStart()
    {
        EventSystem._.OnConfigUpdate += OnConfigUpdate;
        EventSystem._.OnPlayerUnregister += OnPlayerUnregister;
        EventSystem._.OnPlayerSpawn += OnPlayerSpawn;
        EventSystem._.OnPlayerDie += OnPlayerDie;
    }

    protected override void OnLink()
    {
        LoadMaps();
        InitMap(_cfgStartingMap > 0 ? _cfgStartingMap : MapSystem._.MapList.Keys.First(), 1f);
    }

    private void OnDestroy()
    {
        EventSystem._.OnConfigUpdate -= OnConfigUpdate;
        EventSystem._.OnPlayerUnregister -= OnPlayerUnregister;
        EventSystem._.OnPlayerSpawn -= OnPlayerSpawn;
        EventSystem._.OnPlayerDie -= OnPlayerDie;
    }

    // System functions
    public void LoadMaps()
    {
        // Clear any previously loaded maps
        if (MapSystem._.MapList.Count > 0)
        {
            MapSystem._.UnregisterMaps();
        }

        // Load builtin maps
        MapSystem._.RegisterMap(new MapVoid().Generate());
        MapSystem._.RegisterMap(new MapBrutalsky().Generate());
        MapSystem._.RegisterMap(new MapDoomring().Generate());
        MapSystem._.RegisterMap(new MapTossup().Generate());
        MapSystem._.RegisterMap(new MapRacetrack().Generate());

        // Load custom maps
        if (_cfgEnableCustomMaps)
        {
            // MapSystem._.RegisterMaps(ResourceSystem._.LoadFolder("Maps").Select(BsMap.FromLcs));
        }

        // Generate box maps
        if (_cfgEnableBoxMaps)
        {
            var shapes = new byte[] { 0b1000, 0b1011, 0b1111, 0b1100 };
            var sizes = new[] { new Vector2(20f, 10f), new Vector2(40f, 20f), new Vector2(80f, 40f) };
            for (var i = 0; i < shapes.Length; i++) for (var j = 0; j < sizes.Length; j++)
            {
                MapSystem._.RegisterMap(new MapBox(i * sizes.Length + j + 1, shapes[i], sizes[j]).Generate());
            }
        }

        // Generate platformer maps
        if (_cfgEnablePlatformerMaps)
        {
            for (var i = 1; i <= 12; i++)
            {
                MapSystem._.RegisterMap(new MapParkour(i, i, (uint)(i * 0x69), i < 12).Generate());
            }
            MapSystem._.RegisterMap(new MapParkourFinish().Generate());
        }

        // Generate terrain maps
        if (_cfgEnableTerrainMaps)
        {
            // TODO: GENERATE TERRAIN MAPS
        }

        // Generate maze maps
        if (_cfgEnableMazeMaps)
        {
            // TODO: GENERATE MAZE MAPS
        }
    }

    public bool StartRound(uint mapId)
    {
        return mapId != (MapSystem._.ActiveMap?.Id ?? 0)
            ? ChangeMap(mapId, true, transitionTime * 1.5f)
            : RestartRound();
    }

    public bool RestartRound()
    {
        return ChangeMap(0, false, transitionTime);
    }

    public void UpdatePlayerRegistration(BsPlayer player, bool registered)
    {
        if (registered)
        {
            if (!player.Active)
            {
                MapSystem._.RegisterPlayer(player);
            }
        }
        else
        {
            if (player.Active)
            {
                MapSystem._.UnregisterPlayer(player);
            }
        }
    }

    public void InitMap(uint starterMapId, float animationTime)
    {
        MapSystem._.BuildMap(starterMapId);
        MapSystem._.SpawnPlayers();
        var camCover = CameraSystem._.cCameraCover.gameObject;
        camCover.transform.localPosition = Vector2.zero;
        camCover.LeanColor(loadingColor.SetAlpha(0f), animationTime * .4f)
            .setEaseInOutCubic()
            .setOnComplete(() =>
            {
                camCover.transform.localPosition = new Vector2(0f, camCover.transform.localScale.y);
            });
    }

    public bool ChangeMap(uint mapId, bool moveCam, float animationTime)
    {
        if (_mapChangeActive) return false;
        CancelInvoke();

        // Fade out view to hide level loading and fade back in with new level
        var camCover = CameraSystem._.cCameraCover.gameObject;
        _mapChangeActive = true;
        TimeSystem._.ForceUnpause();
        camCover.transform.localPosition = Vector2.zero;
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
                            camCover.transform.localPosition = new Vector2(0f, camCover.transform.localScale.y);
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
    private void OnConfigUpdate(ConfigDelta cfg)
    {
        _cfgStartingMap = (uint)cfg.GetOrDefault("gmmgr", "start", _cfgStartingMap);
        _cfgAutoRestart = (bool)cfg.GetOrDefault("gmmgr", "arest", _cfgAutoRestart);
        _cfgUsePlayer1 = (bool)cfg.GetOrDefault("gmmgr", "uspl1", _cfgUsePlayer1);
        _cfgUsePlayer2 = (bool)cfg.GetOrDefault("gmmgr", "uspl2", _cfgUsePlayer2);
        _cfgEnableCustomMaps = (bool)cfg.GetOrDefault("gmmgr", "encst", _cfgEnableCustomMaps);
        _cfgEnableBoxMaps = (bool)cfg.GetOrDefault("gmmgr", "enbox", _cfgEnableBoxMaps);
        _cfgEnablePlatformerMaps = (bool)cfg.GetOrDefault("gmmgr", "enptf", _cfgEnablePlatformerMaps);
        _cfgEnableTerrainMaps = (bool)cfg.GetOrDefault("gmmgr", "entrn", _cfgEnableTerrainMaps);
        _cfgEnableMazeMaps = (bool)cfg.GetOrDefault("gmmgr", "enmaz", _cfgEnableMazeMaps);

        UpdatePlayerRegistration(_player1, _cfgUsePlayer1);
        UpdatePlayerRegistration(_player2, _cfgUsePlayer2);
    }

    private void OnPlayerUnregister(BsPlayer player)
    {
        _livingPlayers.Remove(player.Id);
    }

    private void OnPlayerSpawn(BsMap map, BsPlayer player, Vector2 position, bool visible)
    {
        if (!visible) return;
        _livingPlayers[player.Id] = player;
    }

    private void OnPlayerDie(BsMap map, BsPlayer player)
    {
        _livingPlayers.Remove(player.Id);
        if (_mapChangeActive) return;
        if ((_cfgAutoRestart && _livingPlayers.Count == 1) || _livingPlayers.Count == 0)
        {
            Invoke(nameof(RestartRound), 3f);
        }
    }
}
