using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Base;
using Controllers.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils.Constants;
using Utils.Map;
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

        // External references
        public GameObject gBackground;
        public GameObject gForeground;
        public Light2D cLight2D;
        private GameObject _gMapParent;

        // Init functions
        protected override void OnStart()
        {
            GenerateMapList(new[] { "Brutalsky", "Doomring", "Voidsky" });
        }

        // System functions
        public void GenerateMapList(string[] builtinMapFilenames)
        {
            RawMapList.Clear();
            foreach (var builtinMapFilename in builtinMapFilenames)
            {
                var builtinRawMap = LoadInternal(builtinMapFilename);
                var builtinMap = BsMap.Parse(builtinRawMap);
                RawMapList[builtinMap.Id] = builtinRawMap;
            }
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}";
            if (!Directory.Exists(path)) return;
            foreach (var mapPath in Directory.GetFiles(path, $"*.{SaveFormat}"))
            {
                var mapFilename = Regex.Match(mapPath, $@"\w+(?=\.{SaveFormat})").Value;
                var rawMap = Load(mapFilename);
                var map = BsMap.Parse(rawMap);
                RawMapList[map.Id] = rawMap;
            }
        }

        public static string LoadInternal(string filename)
        {
            return Resources.Load<TextAsset>($"{Paths.Content}/{Paths.Maps}/{filename}").text;
        }

        public static string Load(string title, string author)
        {
            return Load(GenerateId(title, author).ToString());
        }

        public static string Load(string filename)
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{SaveFormat}";
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        public static void Save(BsMap map)
        {
            Save(map.Stringify(), map.Id.ToString());
        }

        public static void Save(string raw, string title, string author)
        {
            Save(raw, GenerateId(title, author).ToString());
        }

        public static void Save(string raw, string filename)
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{SaveFormat}";
            new FileInfo(path).Directory?.Create();
            using var writer = new StreamWriter(path);
            writer.Write(raw);
        }

        public void Rebuild()
        {
            if (!IsMapLoaded) return;
            var map = ActiveMap;
            Unbuild();
            Build(map);
        }

        public void Build(string title, string author)
        {
            Build(GenerateId(title, author));
        }

        public void Build(uint id)
        {
            Build(BsMap.Parse(RawMapList[id]));
        }

        public void Build(BsMap map)
        {
            // Make sure there is not already an active map
            if (IsMapLoaded) return;

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
                Create(obj);
            }
            EventSystem._.EmitMapBuild(map);
        }

        public void Unbuild()
        {
            // Make sure there is an active map to unbuild
            if (!IsMapLoaded) return;

            // Delete all objects and destroy the map container
            foreach (var obj in ActiveMap.Objects.Values)
            {
                Delete(obj);
            }
            Destroy(_gMapParent);
            _gMapParent = null;
            ActiveMap.ResetSpawns();

            // Note map as inactive
            EventSystem._.EmitMapUnbuild(ActiveMap);
            ActiveMap = null;
            IsMapLoaded = false;
        }

        public bool Create(BsObject obj)
        {
            if (obj.Active) return false;
            var instanceObject = Instantiate(obj.Prefab, _gMapParent.transform);
            var instanceController = obj.Init(instanceObject, ActiveMap);
            obj.Activate(instanceObject, instanceController);
            return true;
        }

        public bool Delete(BsObject obj)
        {
            if (!obj.Active) return false;
            Destroy(obj.InstanceObject);
            obj.Deactivate();
            return true;
        }

        // Utility functions
        public static int Layer2Order(ObjectLayer layer)
        {
            return layer switch
            {
                ObjectLayer.Background => -2,
                ObjectLayer.Foreground => 2,
                _ => 0
            };
        }

        public static Vector2 Gravity2Vector(MapGravity direction, float strength)
        {
            return direction switch
            {
                MapGravity.Down => Vector2.down,
                MapGravity.Up => Vector2.up,
                MapGravity.Left => Vector2.left,
                MapGravity.Right => Vector2.right,
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
