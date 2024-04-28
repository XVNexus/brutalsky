using System;
using System.IO;
using Brutalsky;
using Utils.Joint;
using Utils.Lcs;

namespace Utils.Constants
{
    public static class Errors
    {
        // Lcs format errors
        public static InvalidDataException EmptyLcsDocument()
            => new("Cannot parse an empty LCS document");
        public static InvalidDataException InvalidLcsLine(LcsLine line, int index)
            => new($"Invalid LCS data on line {index + 1}: '{line}'");

        // Core script errors
        public static ArgumentOutOfRangeException InvalidGuiElementType()
            => new("", "Unrecognized GUI element query type");

        public static ArgumentNullException BuildNullMap()
            => new("", "Cannot rebuild the map when no map is currently loaded");

        // Map system errors
        public static ArgumentOutOfRangeException InvalidObjectOrAddonTag(string type, char tag)
            => new("", $"'{tag}' is not a valid {type} tag");
        public static ArgumentOutOfRangeException InvalidJointType(JointType jointType)
            => new(nameof(jointType), $"{jointType} is not a valid joint type");
        public static ArgumentNullException JointMountShapeUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint attached to an unbuilt shape");
    }
}
