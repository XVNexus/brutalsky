using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils.Constants;

namespace Core
{
    public class MapSystem : BsBehavior
    {
        // Static instance
        public static MapSystem _ { get; private set; }
        private void Awake() => _ = this;

        // Local constants
        public const string SaveFormat = "yaml";

        // Local variables
        public Dictionary<uint, string> RawMapList { get; private set; } = new();
        [CanBeNull] public BsMap ActiveMap { get; private set; }
        public bool IsMapLoaded { get; private set; }

        // External references
        public GameObject mapMargins;
        public Light2D cMapLight2D;
        private GameObject _mapParent;

        // Init functions
        protected override void OnStart()
        {
            GenerateMapList();
        }

        // System functions
        public void GenerateMapList()
        {
            RawMapList.Clear();
            var defaultRawMap = LoadInternal("Brutalsky");
            var defaultMap = BsMap.Parse(defaultRawMap);
            RawMapList[defaultMap.Id] = defaultRawMap;
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

        public static string Load(string filename)
        {
            var path = $"{ResourceSystem.DataPath}/{Paths.Maps}/{filename}.{SaveFormat}";
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
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

        public void Build(BsMap map)
        {
            // Make sure there is not already an active map
            if (IsMapLoaded) return;

            // Note map as active
            ActiveMap = map;
            IsMapLoaded = true;

            // Set camera and lighting config
            CameraSystem._.ResizeView(map.Size);
            mapMargins.transform.localScale = map.Size;
            cMapLight2D.color = map.Lighting.Tint;
            cMapLight2D.intensity = map.Lighting.Alpha;

            // Instantiate the map container and create all objects
            _mapParent = new GameObject();
            foreach (var obj in map.Objects.Values)
            {
                Create(obj);
            }
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
            Destroy(_mapParent);
            _mapParent = null;

            // Note map as inactive
            ActiveMap = null;
            IsMapLoaded = false;
        }

        public bool Create(BsObject obj)
        {
            if (obj.Active) return false;
            var instanceObject = Instantiate(obj.Prefab, _mapParent.transform);
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
    }
}
