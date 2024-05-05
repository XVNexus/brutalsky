using System;
using System.IO;
using Brutalsky.Addon;
using Utils.Lcs;

namespace Utils.Constants
{
    public static class Errors
    {
        public static ArgumentOutOfRangeException InvalidItem(string type, object value)
            => new("", $"'{value}' is not a valid {type}");

        public static ArgumentOutOfRangeException NoItemFound(string type, object id)
            => new("", $"No {type} found with id '{id}'");

        public static InvalidOperationException PortCountMismatch(int updateResultCount, int outputPortCount)
            => new($"Cannot apply logic node update of length {updateResultCount} to output ports of length {outputPortCount}");

        public static ArgumentNullException BuildNullMap()
            => new("", "Cannot rebuild the map when no map is currently loaded");

        public static ArgumentNullException JointMountShapeUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint attached to an unbuilt shape");

        public static InvalidDataException EmptyLcsDocument()
            => new("Cannot parse an empty LCS document");

        public static InvalidDataException ErrorParsingLcsLine(LcsLine line, string message)
            => new($"Error while parsing LCS line '{line.Stringify().Trim()}': {message}");
    }
}
