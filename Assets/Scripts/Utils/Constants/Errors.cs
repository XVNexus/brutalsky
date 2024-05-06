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

        public static ArgumentNullException BuildNullMap()
            => new("", "Cannot rebuild the map when no map is currently loaded");

        public static NullReferenceException MissingSubController(string masterTag, string requireeId, string requirementId)
            => new($"Subcontroller '{requireeId}' of object '{masterTag}' requires '{requirementId}'");

        public static ArgumentNullException JointMountUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint attached to an unbuilt shape");

        public static InvalidOperationException PortCountMismatch(int updateResultCount, int outputPortCount)
            => new($"Cannot apply logic node update of length {updateResultCount} to output ports of length {outputPortCount}");

        public static InvalidDataException EmptyLcsDocument()
            => new("Cannot parse an empty LCS document");

        public static InvalidOperationException ErrorWhile(string action, string message)
            => new($"Error while {action}: {message}");

        public static InvalidDataException ErrorWhile(string action, object context, string message)
            => new($"Error while {action} '{context}': {message}");
    }
}
