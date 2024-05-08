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
        public string saveFormatString;
        public string saveFormatBinary;
        public bool useBinaryFormat;

        // External references
        public GameObject pPlayer;
        public GameObject pShape;
        public GameObject pPool;
        public GameObject pSensor;
        public GameObject pMount;
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
                    $@"\d+(?=\.({saveFormatString}|{saveFormatBinary}))").Value)).ToList()
                : new List<BsMap>();
        }

        public BsMap LoadMapFile(string filename)
        {
            var pathBinary = GetDataPath("Maps", filename, saveFormatBinary);
            var pathString = GetDataPath("Maps", filename, saveFormatString);
            BsMap result;
            if (File.Exists(pathBinary))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                result = BsMap.Parse(reader.ReadBytes((int)stream.Length));
            }
            else if (File.Exists(pathString))
            {
                using var reader = new StreamReader(pathString);
                result = BsMap.Parse(reader.ReadToEnd());
            }
            else
            {
                throw Errors.NoItemFound("map file", filename);
            }
            return result;
        }

        public void SaveMapFile(BsMap map)
        {
            if (useBinaryFormat)
            {
                var path = GetDataPath("Maps", map.Id.ToString(), saveFormatBinary);
                using var stream = new FileStream(path, FileMode.Create);
                using var writer = new BinaryWriter(stream);
                writer.Write(map.Binify());
            }
            else
            {
                var path = GetDataPath("Maps", map.Id.ToString(), saveFormatString);
                using var writer = new StreamWriter(path);
                writer.Write(map.Stringify());
            }
        }
    }
}
