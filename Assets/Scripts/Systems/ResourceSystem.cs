using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Controllers.Base;
using Data;
using Data.Base;
using Data.Object;
using Lcs;
using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Systems
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
        public string stringExtension;
        public string binaryExtension;
        public string gzipExtension;
        public byte saveFormat;

        // Local variables
        private string[] _formatExtensions;

        // External references
        public GameObject pPlayer;
        public GameObject pGoal;
        public GameObject pDecal;
        public GameObject pShape;
        public GameObject pJoint;
        public GameObject pPool;
        public GameObject pSensor;
        public GameObject pMount;
        public Material aLitMaterial;
        public Material aUnlitMaterial;

        // Init functions
        protected override void OnStart()
        {
            _formatExtensions = new[] { stringExtension, binaryExtension, gzipExtension };
        }

        // System functions
        public static BsObject GetTemplateObject(string tag)
        {
            return tag switch
            {
                Tags.PlayerPrefix => new BsPlayer(),
                Tags.GoalPrefix => new BsGoal(),
                Tags.DecalPrefix => new BsDecal(),
                Tags.ShapePrefix => new BsShape(),
                Tags.JointPrefix => new BsJoint(),
                Tags.PoolPrefix => new BsPool(),
                Tags.SensorPrefix => new BsSensor(),
                Tags.MountPrefix => new BsMount(),
                _ => throw Errors.InvalidItem("object tag", tag)
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

        public LcsDocument LoadAsset(string directory, string filename)
        {
            return LcsDocument.Parse(Resources.Load<TextAsset>($"Content/{directory}/{filename}").text);
        }

        public LcsDocument[] LoadFolder(string directory)
        {
            var path = GetDataPath(directory);
            return Directory.Exists(path)
                ? Directory.GetFiles(path).Select(filepath => LoadFile(directory, Regex.Match(filepath,
                    $@"\d+(?=\.({string.Join('|', _formatExtensions)}))").Value)).ToArray()
                : throw Errors.NoItemFound("document folder", directory);
        }

        public LcsDocument LoadFile(string directory, string filename)
        {
            var pathString = GetDataPath(directory, filename, stringExtension);
            var pathBinary = GetDataPath(directory, filename, binaryExtension);
            var pathGzip = GetDataPath(directory, filename, gzipExtension);
            if (File.Exists(pathString))
            {
                using var reader = new StreamReader(pathString);
                return LcsDocument.Parse(reader.ReadToEnd());
            }
            if (File.Exists(pathBinary))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                return LcsDocument.Parse(reader.ReadBytes((int)stream.Length));
            }
            if (File.Exists(pathGzip))
            {
                using var stream = new FileStream(pathBinary, FileMode.Open);
                using var reader = new BinaryReader(stream);
                return LcsDocument.Parse(reader.ReadBytes((int)stream.Length), true);
            }
            throw Errors.NoItemFound("document file", $"{directory}/{filename}");
        }

        public void SaveFile(string directory, string filename, LcsDocument document, byte? format = null)
        {
            switch (format ?? saveFormat)
            {
                case LcsDocument.FormatString:
                {
                    var pathString = GetDataPath(directory, filename, stringExtension);
                    using var writer = new StreamWriter(pathString);
                    writer.Write(document.Stringify());
                    break;
                }
                case LcsDocument.FormatBinary:
                {
                    var pathGzip = GetDataPath(directory, filename, binaryExtension);
                    using var stream = new FileStream(pathGzip, FileMode.Create);
                    using var writer = new BinaryWriter(stream);
                    writer.Write(document.Binify());
                    break;
                }
                case LcsDocument.FormatGzip:
                {
                    var pathGzip = GetDataPath(directory, filename, gzipExtension);
                    using var stream = new FileStream(pathGzip, FileMode.Create);
                    using var writer = new BinaryWriter(stream);
                    writer.Write(document.Binify(true));
                    break;
                }
                default:
                    throw Errors.InvalidItem("lcs format", saveFormat);
            }
        }

        public bool HasFolder(string directory)
        {
            return Directory.Exists(GetDataPath(directory));
        }

        public bool HasFile(string directory, string filename)
        {
            return File.Exists(GetDataPath(directory, filename, stringExtension))
                || File.Exists(GetDataPath(directory, filename, binaryExtension))
                || File.Exists(GetDataPath(directory, filename, gzipExtension));
        }
    }
}
