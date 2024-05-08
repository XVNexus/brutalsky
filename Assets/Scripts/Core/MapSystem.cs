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
using Utils.Player;

namespace Core
{
    public class MapSystem : BsBehavior
    {
        // Static instance
        public static MapSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Config options
        public float backgroundFade;
        public float backgroundField;
        public string saveFormatString;
        public string saveFormatBinary;
        public bool useBinaryFormat;

        // Exposed properties
        // Maps are saved as gzip binary to not take up 2 terabytes of memory
        public Dictionary<uint, byte[]> MapList { get; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public Dictionary<string, BsPlayer> ActivePlayers { get; } = new();
        [CanBeNull] public BsMatrix Matrix { get; private set; }
        public bool MapLoaded { get; private set; }

        // External references
        public GameObject gMapParent;
        public GameObject gPlayerParent;
        public SpriteRenderer gBackgroundMain;
        public SpriteRenderer[] gBackgroundEdges;
        public SpriteRenderer[] gBackgroundCorners;
        public SpriteRenderer[] gBackgroundOobs;
        public Light2D cLight2D;

        // System functions
        public void RegisterPlayers(IEnumerable<BsPlayer> players)
        {
            foreach (var player in players)
            {
                player.Activate(gPlayerParent.transform, ActiveMap);
                ActivePlayers[player.Id] = player;
            }
        }

        public void UnregisterPlayers()
        {
            foreach (var player in ActivePlayers.Values)
            {
                player.Deactivate();
            }
            ActivePlayers.Clear();
        }

        public void SpawnPlayers()
        {
            var playerList = ActivePlayers.Values.ToList();
            while (playerList.Count > 0)
            {
                var index = ResourceSystem.Random.NextInt(playerList.Count);
                var player = playerList[index];
                SpawnPlayer(player, ActiveMap.AllowDummies || player.Type != PlayerType.Dummy);
                playerList.RemoveAt(index);
            }
        }

        public void SpawnPlayer(BsPlayer player, bool visible = true)
        {
            SetPlayerFrozen(player.InstanceObject, !visible);
            var position = visible ? ActiveMap.SelectSpawn() : new Vector2(1e6f, 1e6f);
            player.Health = ActiveMap.PlayerHealth;
            player.InstanceObject.transform.localPosition = position;
            EventSystem._.EmitPlayerSpawn(ActiveMap, player, position, visible);
        }

        public bool GetPlayerFrozen(GameObject playerInstanceObject)
        {
            return !playerInstanceObject.GetComponent<Rigidbody2D>().simulated;
        }

        public void SetPlayersFrozen(bool frozen, bool resetVelocity = false)
        {
            foreach (var player in ActivePlayers.Values)
            {
                SetPlayerFrozen(player.InstanceObject, frozen, resetVelocity);
            }
        }

        public void SetPlayerFrozen(GameObject playerInstanceObject, bool frozen, bool resetVelocity = false)
        {
            var rigidbody = playerInstanceObject.GetComponent<Rigidbody2D>();
            rigidbody.simulated = !frozen;
            playerInstanceObject.GetComponent<CircleCollider2D>().enabled = !frozen;
            if (!resetVelocity) return;
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
        }

        public void LoadMapAssets(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                LoadMapAsset(filename);
            }
        }

        public void LoadMapAsset(string filename)
        {
            RegisterMap(BsMap.Parse(Resources.Load<TextAsset>($"{Paths.Content}/{Paths.Maps}/{filename}").text));
        }

        public void LoadMapFiles()
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}";
            if (!Directory.Exists(path)) return;
            foreach (var mapPath in Directory.GetFiles(path))
            {
                LoadMapFile(Regex.Match(mapPath, $@"\d+(?=\.({saveFormatString}|{saveFormatBinary}))").Value);
            }
        }

        public void LoadMapFile(string filename)
        {
            var pathBinary = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{saveFormatBinary}";
            var pathString = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{saveFormatString}";
            BsMap map;
            if (File.Exists(pathBinary))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                map = BsMap.Parse(reader.ReadBytes((int)stream.Length));
            }
            else if (File.Exists(pathString))
            {
                using var reader = new StreamReader(pathString);
                map = BsMap.Parse(reader.ReadToEnd());
            }
            else
            {
                throw Errors.NoItemFound("map file", filename);
            }
            RegisterMap(map);
        }

        public void SaveMapFile(BsMap map)
        {
            if (useBinaryFormat)
            {
                var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{map.Id}.{saveFormatBinary}";
                new FileInfo(path).Directory?.Create();
                using var stream = new FileStream(path, FileMode.Create);
                using var writer = new BinaryWriter(stream);
                writer.Write(map.Binify());
            }
            else
            {
                var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{map.Id}.{saveFormatString}";
                new FileInfo(path).Directory?.Create();
                using var writer = new StreamWriter(path);
                writer.Write(map.Stringify());
            }
        }

        public void RegisterMap(BsMap map)
        {
            EventSystem._.EmitMapPreload(map);
            MapList[map.Id] = map.Binify();
        }

        public void UnregisterMaps()
        {
            EventSystem._.EmitMapsUnload();
            MapList.Clear();
        }

        public void BuildMap(string title, string author)
        {
            BuildMap(GenerateId(title, author));
        }

        public void BuildMap(uint id)
        {
            if (id > 0)
            {
                BuildMap(BsMap.Parse(MapList[id]));
            }
            else
            {
                BuildMap();
            }
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
            CameraSystem._.SetBaseRect(map.PlayArea);
            var backgroundColor = map.BackgroundColor;
            var halfArea = map.PlayArea.size * .5f;
            var halfFade = backgroundFade * .5f;
            var halfField = backgroundField * .5f;
            gBackgroundMain.transform.parent.localPosition = map.PlayArea.center;
            gBackgroundMain.color = backgroundColor;
            gBackgroundMain.transform.localScale = map.PlayArea.size;
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundEdge = gBackgroundEdges[i];
                gBackgroundEdge.color = backgroundColor;
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
                    0 => new Vector2(map.PlayArea.width, backgroundFade),
                    1 => new Vector2(map.PlayArea.height, backgroundFade),
                    _ => gBackgroundEdge.transform.localScale
                };
            }
            for (var i = 0; i < 8; i++)
            {
                var gBackgroundCorner = gBackgroundCorners[i];
                gBackgroundCorner.color = backgroundColor;
                gBackgroundCorner.transform.localPosition = (i % 4) switch
                {
                    0 => new Vector2(halfArea.x + halfFade, halfArea.y + halfFade),
                    1 => new Vector2(halfArea.x + halfFade, -halfArea.y - halfFade),
                    2 => new Vector2(-halfArea.x - halfFade, -halfArea.y - halfFade),
                    3 => new Vector2(-halfArea.x - halfFade, halfArea.y + halfFade),
                    _ => gBackgroundCorner.transform.localPosition
                };
                gBackgroundCorner.transform.localScale = new Vector2(backgroundFade, backgroundFade);
            }
            for (var i = 0; i < 4; i++)
            {
                var gBackgroundOob = gBackgroundOobs[i];
                gBackgroundOob.color = backgroundColor;
                gBackgroundOob.transform.localPosition = i switch
                {
                    0 => new Vector2(0f, halfArea.y + backgroundFade + halfField),
                    1 => new Vector2(halfArea.x + backgroundFade + halfField, 0f),
                    2 => new Vector2(0f, -halfArea.y - backgroundFade - halfField),
                    3 => new Vector2(-halfArea.x - backgroundFade - halfField, 0f),
                    _ => gBackgroundOob.transform.localPosition
                };
                gBackgroundOob.transform.localScale = new Vector2(backgroundField, backgroundField);
            }
            cLight2D.color = map.LightingTint;
            cLight2D.intensity = map.LightingIntensity;
            Physics2D.gravity = GravityToVector(map.GravityDirection, map.GravityStrength);

            // Create all objects
            foreach (var obj in map.Objects.Values)
            {
                CreateObject(obj);
            }

            Invoke(nameof(StartMap), 0f);
        }

        private void StartMap()
        {
            // Start the logic system
            var logicNodes = new List<BsNode>();
            logicNodes.AddRange(ActiveMap.Nodes);
            EventSystem._.EmitMapBuild(ActiveMap);
            foreach (var obj in ActiveMap.Objects.Values)
            {
                logicNodes.AddRange(obj.RegisterLogic());
            }
            Matrix = new BsMatrix(logicNodes, ActiveMap.Links.Values);
        }

        public void UnbuildMap()
        {
            // Make sure there is an active map to unbuild
            if (!MapLoaded) return;

            // Give everything a chance to remove any trash that might get left behind
            EventSystem._.EmitMapCleanup(ActiveMap);

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
            obj.Deactivate();
        }

        // Event functions
        private void FixedUpdate()
        {
            Matrix?.Update();
        }

        // Utility functions
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
