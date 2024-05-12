using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Brutalsky;
using Brutalsky.Addon;
using Brutalsky.Base;
using Brutalsky.Object;
using Controllers.Base;
using UnityEngine;
using Utils.Constants;
using Utils.Lcs;
using Random = Unity.Mathematics.Random;

namespace Core
{
    public class ResourceSystem : BsBehavior
    {
        // Static instance
        public static ResourceSystem _ { get; private set; }
        public static Random Random;
        public static string DataPath;

        private void Awake()
        {
            _ = this;
            Random = Random.CreateFromIndex((uint)DateTime.UtcNow.Ticks);
            DataPath = Application.persistentDataPath;
            foreach (var dataFolder in dataDirectories)
            {
                new DirectoryInfo($"{DataPath}/{dataFolder}").Create();
            }
        }

        // Config options
        public string[] dataDirectories;
        public string[] lcsFormatExtensions;
        public LcsFormat lcsPreferredFormat;

        // External references
        public GameObject pPlayer;
        public GameObject pShape;
        public GameObject pPool;
        public GameObject pSensor;
        public GameObject pMount;
        public GameObject pGoal;
        public Material aLitMaterial;
        public Material aUnlitMaterial;

        // System functions
        public static BsObject GetTemplateObject(string tag)
        {
            return tag switch
            {
                Tags.PlayerPrefix => new BsPlayer(),
                Tags.ShapePrefix => new BsShape(),
                Tags.PoolPrefix => new BsPool(),
                Tags.SensorPrefix => new BsSensor(),
                Tags.MountPrefix => new BsMount(),
                Tags.GoalPrefix => new BsGoal(),
                _ => throw Errors.InvalidItem("object tag", tag)
            };
        }

        public static BsAddon GetTemplateAddon(string tag)
        {
            return tag switch
            {
                Tags.JointPrefix => new BsJoint(),
                _ => throw Errors.InvalidItem("addon tag", tag)
            };
        }

        public static string GetDataPath(string directory)
        {
            return $"{DataPath}/{directory}";
        }

        public static string GetDataPath(string directory, string filename)
        {
            return $"{DataPath}/{directory}/{filename}";
        }

        public static string GetDataPath(string directory, string filename, string extension)
        {
            return $"{DataPath}/{directory}/{filename}.{extension}";
        }

        public static T LoadContent<T>(string directory, string filename) where T : UnityEngine.Object
        {
            return Resources.Load<T>($"Content/{directory}/{filename}");
        }

        public IEnumerable<BsMap> LoadMapAssets(IEnumerable<string> filenames)
        {
            return filenames.Select(filename => LoadMapAsset(filename)).ToList();
        }

        public BsMap LoadMapAsset(string filename)
        {
            return BsMap.Parse(LoadContent<TextAsset>("Maps", filename).text);
        }

        public IEnumerable<BsMap> LoadMapFiles()
        {
            var path = GetDataPath("Maps");
            return Directory.Exists(path)
                ? Directory.GetFiles(path).Select(mapPath => LoadMapFile(Regex.Match(mapPath,
                    $@"\d+(?=\.({string.Join('|', lcsFormatExtensions)}))").Value)).ToList()
                : new List<BsMap>();
        }

        public BsMap LoadMapFile(string filename)
        {
            var pathString = GetDataPath("Maps", filename, lcsFormatExtensions[0]);
            var pathBinary = GetDataPath("Maps", filename, lcsFormatExtensions[1]);
            var pathGzip = GetDataPath("Maps", filename, lcsFormatExtensions[2]);
            if (File.Exists(pathString))
            {
                using var reader = new StreamReader(pathString);
                return BsMap.Parse(reader.ReadToEnd());
            }
            if (File.Exists(pathBinary))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                return BsMap.Parse(reader.ReadBytes((int)stream.Length), false);
            }
            if (File.Exists(pathGzip))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                return BsMap.Parse(reader.ReadBytes((int)stream.Length));
            }
            throw Errors.NoItemFound("map file", filename);
        }

        public void SaveMapFile(BsMap map)
        {
            switch (lcsPreferredFormat)
            {
                case LcsFormat.String:
                {
                    var pathString = GetDataPath("Maps", map.Id.ToString(), lcsFormatExtensions[0]);
                    using var writer = new StreamWriter(pathString);
                    writer.Write(map.Stringify());
                    break;
                }
                case LcsFormat.Binary:
                {
                    var pathGzip = GetDataPath("Maps", map.Id.ToString(), lcsFormatExtensions[1]);
                    using var stream = new FileStream(pathGzip, FileMode.Create);
                    using var writer = new BinaryWriter(stream);
                    writer.Write(map.Binify());
                    break;
                }
                case LcsFormat.Gzip:
                {
                    var pathGzip = GetDataPath("Maps", map.Id.ToString(), lcsFormatExtensions[2]);
                    using var stream = new FileStream(pathGzip, FileMode.Create);
                    using var writer = new BinaryWriter(stream);
                    writer.Write(map.Binify(true));
                    break;
                }
                default:
                    throw Errors.InvalidItem("lcs format", lcsPreferredFormat);
            }
        }
    }
}
