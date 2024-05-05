using System.Collections.Generic;

namespace Utils.Lcs
{
    public class LcsProp
    {
        public static readonly Dictionary<LcsType, char> TypeCharTable = new()
        {
            { LcsType.Bool, 'b' },
            { LcsType.UShort, 'h' },
            { LcsType.UInt, 'n' },
            { LcsType.ULong, 'o' },
            { LcsType.Short, 's' },
            { LcsType.Int, 'i' },
            { LcsType.Long, 'l' },
            { LcsType.Float, 'f' },
            { LcsType.Double, 'd' },
            { LcsType.Char, 'c' },
            { LcsType.String, 't' },
            { LcsType.Direction, 'D' },
            { LcsType.Layer, 'L' },
            { LcsType.FormType, 'F' },
            { LcsType.JointType, 'J' },
            { LcsType.Vector2, 'V' },
            { LcsType.Color, 'C' },
            { LcsType.Transform, 'T' },
            { LcsType.Form, 'O' },
            { LcsType.Material, 'M' },
            { LcsType.Chemical, 'H' }
        };
        public static readonly Dictionary<char, LcsType> CharTypeTable = new()
        {
            { 'b', LcsType.Bool },
            { 'h', LcsType.UShort },
            { 'n', LcsType.UInt },
            { 'o', LcsType.ULong },
            { 's', LcsType.Short },
            { 'i', LcsType.Int },
            { 'l', LcsType.Long },
            { 'f', LcsType.Float },
            { 'd', LcsType.Double },
            { 'c', LcsType.Char },
            { 't', LcsType.String },
            { 'D', LcsType.Direction },
            { 'L', LcsType.Layer },
            { 'F', LcsType.FormType },
            { 'J', LcsType.JointType },
            { 'V', LcsType.Vector2 },
            { 'C', LcsType.Color },
            { 'T', LcsType.Transform },
            { 'O', LcsType.Form },
            { 'M', LcsType.Material },
            { 'H', LcsType.Chemical }
        };

        public LcsType Type { get; set; }
        public object Value { get; set; }

        public LcsProp(LcsType type, object value)
        {
            Type = type;
            Value = value;
        }

        public string Stringify()
        {
            return $"{TypeCharTable[Type]}{Stringifier.Stringify(Type, Value)}";
        }

        public static LcsProp Parse(string raw)
        {
            var type = CharTypeTable[raw[0]];
            var value = Stringifier.Parse(type, raw[1..]);
            return new LcsProp(type, value);
        }

        public override string ToString()
        {
            return Stringify();
        }
    }
}
