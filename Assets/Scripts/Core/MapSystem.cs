using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Base;
using Controllers;
using Controllers.Base;
using Controllers.Player;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils.Constants;
using Utils.Object;

namespace Core
{
    public class MapSystem : BsBehavior
    {
        // Static instance
        public static MapSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local constants
        public const string SaveFormat = "txt";

        // Local variables
        public Dictionary<uint, string> RawMapList { get; private set; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public bool IsMapLoaded { get; private set; }
        public Dictionary<string, BsPlayer> ActivePlayers { get; private set; } = new();

        // External references
        public GameObject gBackground;
        public GameObject gForeground;
        public Light2D cLight2D;
        private GameObject _gMapParent;

        // System functions
        public void ScanMapFiles(string[] builtinMapFilenames)
        {
            RawMapList.Clear();
            foreach (var builtinMapFilename in builtinMapFilenames)
            {
                var builtinRawMap = LoadInternalMap(builtinMapFilename);
                var builtinMap = BsMap.Parse(builtinRawMap);
                RawMapList[builtinMap.Id] = builtinRawMap;
            }
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}";
            if (!Directory.Exists(path)) return;
            foreach (var mapPath in Directory.GetFiles(path, $"*.{SaveFormat}"))
            {
                var mapFilename = Regex.Match(mapPath, $@"\w+(?=\.{SaveFormat})").Value;
                var rawMap = LoadMap(mapFilename);
                var map = BsMap.Parse(rawMap);
                RawMapList[map.Id] = rawMap;
            }
        }

        public void RegisterPlayers(IEnumerable<BsPlayer> players)
        {
            foreach (var player in players)
            {
                ActivePlayers[player.Id] = player;
            }
        }

        public void BuildMap(string title, string author)
        {
            BuildMap(GenerateId(title, author));
        }

        public void BuildMap(uint id)
        {
            BuildMap(BsMap.Parse(RawMapList[id]));
        }

        public void BuildMap([CanBeNull] BsMap map = null)
        {
            // If the map parameter is null, rebuild the current map
            if (map == null)
            {
                map = ActiveMap ?? throw Errors.BuildNullMap();
            }

            // Make sure there is not already an active map
            if (IsMapLoaded)
            {
                UnbuildMap();
            }

            // Note map as active
            ActiveMap = map;
            IsMapLoaded = true;

            // Apply config
            CameraSystem._.ResizeView(map.PlayArea);
            gBackground.transform.localScale = map.PlayArea;
            gForeground.transform.localScale = map.PlayArea;
            var backgroundColor = map.BackgroundColor.Tint * map.LightingColor.Tint;
            gBackground.GetComponent<SpriteRenderer>().color = backgroundColor;
            gForeground.GetComponent<SpriteRenderer>().color = backgroundColor;
            cLight2D.color = map.LightingColor.Tint;
            cLight2D.intensity = map.LightingColor.Alpha;
            Physics2D.gravity = Gravity2Vector(map.GravityDirection, map.GravityStrength);

            // Instantiate the map container and create all objects
            _gMapParent = new GameObject();
            foreach (var obj in map.Objects.Values)
            {
                CreateObject(obj);
            }
            EventSystem._.EmitMapBuild(map);
        }

        public void UnbuildMap()
        {
            // Make sure there is an active map to unbuild
            if (!IsMapLoaded) return;

            // Delete all objects and destroy the map container
            foreach (var obj in ActiveMap.Objects.Values)
            {
                DeleteObject(obj);
            }
            Destroy(_gMapParent);
            _gMapParent = null;
            ActiveMap.ResetSpawns();

            // Note map as inactive
            EventSystem._.EmitMapUnbuild(ActiveMap);
            ActiveMap = null;
            IsMapLoaded = false;
        }

        public bool CreateObject(BsObject obj)
        {
            if (obj.Active) return false;
            var instanceObject = Instantiate(obj.Prefab, _gMapParent.transform);
            var instanceController = obj.Init(instanceObject, ActiveMap);
            obj.Activate(instanceObject, instanceController);
            return true;
        }

        public bool DeleteObject(BsObject obj)
        {
            if (!obj.Active) return false;
            Destroy(obj.InstanceObject);
            obj.Deactivate();
            return true;
        }

        public void FreezeAllPlayers()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceController)?.GetSub<PlayerMovementController>("movement")?.Freeze();
            }
        }

        public void UnfreezeAllPlayers()
        {
            foreach (var player in ActivePlayers.Values)
            {
                ((PlayerController)player.InstanceController)?.GetSub<PlayerMovementController>("movement")?.Unfreeze();
            }
        }

        public void SpawnAllPlayers()
        {
            var playerList = ActivePlayers.Values.ToList();
            while (playerList.Count > 0)
            {
                var index = ResourceSystem.Random.NextInt(playerList.Count);
                SpawnPlayer(playerList[index]);
                playerList.RemoveAt(index);
            }
        }

        public void DespawnAllPlayers()
        {
            var playerList = ActivePlayers.Values.ToList();
            foreach (var player in playerList)
            {
                DespawnPlayer(player);
            }
        }

        public void SpawnPlayer(BsPlayer player)
        {
            if (!IsMapLoaded) return;
            player.Health = ActiveMap.PlayerHealth;
            if (!player.Active)
            {
                // Spawn new player
                var instanceObject = Instantiate(player.Prefab);
                var instanceController = player.Init(instanceObject, ActiveMap);
                player.Activate(instanceObject, instanceController);
                ActivePlayers[player.Id] = player;
                instanceObject.transform.position = ActiveMap.SelectSpawn();
                EventSystem._.EmitPlayerSpawn(ActiveMap, player);
            }
            else
            {
                // Respawn existing player
                player.InstanceObject.transform.position = ActiveMap.SelectSpawn();
                EventSystem._.EmitPlayerRespawn(ActiveMap, player);
            }
        }

        public void DespawnPlayer(BsPlayer player)
        {
            if (!player.Active || !IsMapLoaded) return;
            Destroy(player.InstanceObject);
            player.Deactivate();
            ActivePlayers.Remove(player.Id);
        }

        // Utility functions
        public static string LoadInternalMap(string filename)
        {
            return Resources.Load<TextAsset>($"{Paths.Content}/{Paths.Maps}/{filename}").text;
        }

        public static string LoadMap(string title, string author)
        {
            return LoadMap(GenerateId(title, author).ToString());
        }

        public static string LoadMap(string filename)
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{SaveFormat}";
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        public static void SaveMap(BsMap map)
        {
            SaveMap(map.Stringify(), map.Id.ToString());
        }

        public static void SaveMap(string raw, string title, string author)
        {
            SaveMap(raw, GenerateId(title, author).ToString());
        }

        public static void SaveMap(string raw, string filename)
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{SaveFormat}";
            new FileInfo(path).Directory?.Create();
            using var writer = new StreamWriter(path);
            writer.Write(raw);
        }

        public static int Layer2Order(ObjectLayer layer)
        {
            return layer switch
            {
                ObjectLayer.Background => -2,
                ObjectLayer.Foreground => 2,
                _ => 0
            };
        }

        public static Vector2 Gravity2Vector(Direction direction, float strength)
        {
            return direction switch
            {
                Direction.Down => Vector2.down,
                Direction.Up => Vector2.up,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => Vector2.zero
            } * strength;
        }

        public static string CleanId(string id)
        {
            return Regex.Replace(id.Replace(' ', '-').ToLower(), "[^a-z0-9-]|-(?=-)", "");
        }

        public static uint GenerateId(string title, string author)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
        }
    }
}
