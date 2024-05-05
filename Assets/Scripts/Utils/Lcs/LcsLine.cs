using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Utils.Constants;
using Utils.Joint;
using Utils.Object;
using Utils.Pool;
using Utils.Shape;

namespace Utils.Lcs
{
    public class LcsLine
    {
        public char Prefix { get; set; }
        public string[] Properties { get; set; }
        public List<LcsLine> Children { get; set; }

        public LcsLine(char prefix, string[] properties, [CanBeNull] List<LcsLine> children = null)
        {
            Prefix = prefix;
            Properties = properties;
            Children = children ?? new List<LcsLine>();
        }

        public static LcsLine Parse(string raw)
        {
            var prefix = raw[0];
            var properties = Stringifier.ExpandProperties(raw[1..]);
            return new LcsLine(prefix, properties);
        }

        public string Stringify()
        {
            var result = $"{Prefix}{Stringifier.CompressProperties(Properties)}\n";
            result = Children.Aggregate(result, (current, child) => current + child.Stringify());
            return result;
        }

        public override string ToString()
        {
            return $"{Prefix}{Stringifier.CompressProperties(Properties)}";
        }

        public static Type GetType(LcsType type)
        {
            return type switch
            {
                LcsType.Bool => typeof(bool),
                LcsType.UShort => typeof(ushort),
                LcsType.UInt => typeof(uint),
                LcsType.ULong => typeof(ulong),
                LcsType.Short => typeof(short),
                LcsType.Int => typeof(int),
                LcsType.Long => typeof(long),
                LcsType.Float => typeof(float),
                LcsType.Double => typeof(double),
                LcsType.Char => typeof(char),
                LcsType.String => typeof(string),
                LcsType.Direction => typeof(Direction),
                LcsType.Layer => typeof(ObjectLayer),
                LcsType.FormType => typeof(FormType),
                LcsType.JointType => typeof(JointType),
                LcsType.Vector2 => typeof(Vector2),
                LcsType.Color => typeof(Color),
                LcsType.Transform => typeof(ObjectTransform),
                LcsType.Form => typeof(Form),
                LcsType.Material => typeof(ShapeMaterial),
                LcsType.Chemical => typeof(PoolChemical),
                _ => throw Errors.InvalidItem("LCS type", type)
            };
        }
    }
}
