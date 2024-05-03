using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Base;
using Brutalsky.Logic;
using Brutalsky.Object;
using Controllers.Base;
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
        public const float BackgroundFade = 10f;
        public const float BackgroundField = 1000f;

        // Local variables
        public Dictionary<uint, string> RawMapList { get; private set; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public Dictionary<string, BsPlayer> ActivePlayers { get; private set; } = new();
        [CanBeNull] public BsMatrix Matrix { get; private set; }
        public bool MapLoaded { get; private set; }

        // External references
        public GameObject gMapParent;
        public GameObject gBackgroundMain;
        public GameObject[] gBackgroundEdges;
        public GameObject[] gBackgroundCorners;
        public GameObject[] gBackgroundOobs;
        public Light2D cLight2D;

        // System functions
        public void ResaveBuiltinMaps(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                var rawMap = LoadInternalMap(filename);
                var map = BsMap.Parse(rawMap);
                SaveMap(map);
            }
        }

        public void ScanMapFiles()
        {
            RawMapList.Clear();
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
                player.Activate(gMapParent.transform, ActiveMap);
                ActivePlayers[player.Id] = player;
            }
        }

        public void UnregisterPlayers()
        {
            foreach (var id in ActivePlayers.Keys)
            {
                ActivePlayers[id].Deactivate(ActiveMap);
                ActivePlayers.Remove(id);
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
            // If the map parameter is null, build the current map
            if (map == null)
            {
                map = ActiveMap ?? throw Errors.BuildNullMap();
            }

            // Make sure there is not already an active map
            if (MapLoaded)
            {
                UnbuildMap();
            }

            // Note map as active
            ActiveMap = map;
            MapLoaded = true;

            // Apply config
            CameraSystem._.ResizeView(map.PlayArea);
            var backgroundColor = map.BackgroundColor.Tint * map.LightingColor.Tint;
            var halfArea = map.PlayArea * .5f;
            const float halfFade = BackgroundFade * .5f;
            const float halfField = BackgroundField * .5f;
            gBackgroundMain.GetComponent<SpriteRenderer>().color = backgroundColor;
            gBackgroundMain.transform.localScale = map.PlayArea;
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundEdge = gBackgroundEdges[i];
                gBackgroundEdge.GetComponent<SpriteRenderer>().color = backgroundColor;
                gBackgroundEdge.transform.localPosition = (i % 4) switch
                {
                    0 => new Vector2(0f, halfArea.y + halfFade),
                    1 => new Vector2(halfArea.x + halfFade, 0f),
                    2 => new Vector2(0f, -halfArea.y - halfFade),
                    3 => new Vector2(-halfArea.x - halfFade, 0f),
                    _ => gBackgroundEdge.transform.localPosition
                };
                gBackgroundEdge.transform.localScale = (i % 2) switch
                {
                    0 => new Vector2(map.PlayArea.x, BackgroundFade),
                    1 => new Vector2(map.PlayArea.y, BackgroundFade),
                    _ => gBackgroundEdge.transform.localScale
                };
            }
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundCorner = gBackgroundCorners[i];
                gBackgroundCorner.GetComponent<SpriteRenderer>().color = backgroundColor;
                gBackgroundCorner.transform.localPosition = (i % 4) switch
                {
                    0 => new Vector2(halfArea.x + halfFade, halfArea.y + halfFade),
                    1 => new Vector2(halfArea.x + halfFade, -halfArea.y - halfFade),
                    2 => new Vector2(-halfArea.x - halfFade, -halfArea.y - halfFade),
                    3 => new Vector2(-halfArea.x - halfFade, halfArea.y + halfFade),
                    _ => gBackgroundCorner.transform.localPosition
                };
                gBackgroundCorner.transform.localScale = new Vector2(BackgroundFade, BackgroundFade);
            }
            for (var i = 0; i < 4; i++)
            {
                var gBackgroundOob = gBackgroundOobs[i];
                gBackgroundOob.GetComponent<SpriteRenderer>().color = backgroundColor;
                gBackgroundOob.transform.localPosition = i switch
                {
                    0 => new Vector2(0f, halfArea.y + BackgroundFade + halfField),
                    1 => new Vector2(halfArea.x + BackgroundFade + halfField, 0f),
                    2 => new Vector2(0f, -halfArea.y - BackgroundFade - halfField),
                    3 => new Vector2(-halfArea.x - BackgroundFade - halfField, 0f),
                    _ => gBackgroundOob.transform.localPosition
                };
                gBackgroundOob.transform.localScale = new Vector2(BackgroundField, BackgroundField);
            }
            cLight2D.color = map.LightingColor.Tint;
            cLight2D.intensity = map.LightingColor.Alpha;
            Physics2D.gravity = GravityToVector(map.GravityDirection, map.GravityStrength);

            // Create all objects
            var logicNodes = new List<BsNode>();
            foreach (var obj in map.Objects.Values)
            {
                CreateObject(obj);
                logicNodes.AddRange(obj.RegisterLogic());
            }
            logicNodes.AddRange(map.Nodes);
            EventSystem._.EmitMapBuild(map);

            // Start the logic system
            Matrix = new BsMatrix(logicNodes, map.Links.Values);
        }

        public void UnbuildMap()
        {
            // Make sure there is an active map to unbuild
            if (!MapLoaded) return;

            // Delete all objects and destroy the map container
            foreach (var obj in ActiveMap.Objects.Values)
            {
                DeleteObject(obj);
            }
            ActiveMap.ResetSpawns();

            // Stop the logic system
            Matrix = null;

            // Note map as inactive
            EventSystem._.EmitMapUnbuild(ActiveMap);
            ActiveMap = null;
            MapLoaded = false;
        }

        public void CreateObject(BsObject obj)
        {
            if (obj.Active) return;
            obj.Activate(gMapParent.transform, ActiveMap);
        }

        public void DeleteObject(BsObject obj)
        {
            if (!obj.Active) return;
            obj.Deactivate(ActiveMap);
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

        public void SpawnPlayer(BsPlayer player)
        {
            if (!MapLoaded || !player.Active) return;
            player.Health = ActiveMap.PlayerHealth;
            player.InstanceObject.transform.position = ActiveMap.SelectSpawn();
            EventSystem._.EmitPlayerSpawn(ActiveMap, player);
        }

        // Event functions
        private void FixedUpdate()
        {
            Matrix?.Update();
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

        public static int LayerToOrder(ObjectLayer layer)
        {
            return layer switch
            {
                ObjectLayer.Background => -2,
                ObjectLayer.Foreground => 2,
                _ => 0
            };
        }

        public static Vector2 GravityToVector(Direction direction, float strength)
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
            return Regex.Replace(id.Replace(' ', '-').ToLower(), "[^a-z0-9-.]|-(?=-)", "");
        }

        public static uint GenerateId(string title, string author)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
        }
    }
}
