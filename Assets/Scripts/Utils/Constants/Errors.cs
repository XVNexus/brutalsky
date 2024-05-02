using System;
using System.IO;
using Brutalsky.Addon;
using Utils.Joint;
using Utils.Lcs;

namespace Utils.Constants
{
    public static class Errors
    {
        public static InvalidDataException EmptyLcsDocument()
            => new("Cannot parse an empty LCS document");

        public static InvalidDataException InvalidLcsLine(LcsLine line, int index)
            => new($"Invalid LCS data on line {index + 1}: '{line}'");

        public static ArgumentOutOfRangeException InvalidGuiElementType()
            => new("", "Unrecognized GUI element query type");

        public static ArgumentNullException BuildNullMap()
            => new("", "Cannot rebuild the map when no map is currently loaded");

        public static InvalidOperationException PortCountMismatch(int updateResultCount, int outputPortCount)
            => new($"Cannot apply logic node update of length {updateResultCount} to output ports of length {outputPortCount}");

        public static ArgumentOutOfRangeException NoNodeFound(int id)
            => new("", $"No logic node found with id '{id}'");

        public static ArgumentOutOfRangeException NoPortFound(string type, int id, int index)
            => new("", $"No logic {type} port found on node '{id}' with index '{index}'");

        public static ArgumentOutOfRangeException InvalidObjectOrAddonTag(string type, string tag)
            => new("", $"'{tag}' is not a valid {type} tag");

        public static ArgumentOutOfRangeException InvalidJointType(JointType jointType)
            => new(nameof(jointType), $"'{jointType}' is not a valid joint type");

        public static ArgumentNullException JointMountShapeUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint attached to an unbuilt shape");
    }
}
