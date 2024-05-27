using System;
using System.IO;
using Data.Object;
using UnityEngine;

namespace Utils.Constants
{
    public static class Errors
    {
        public static ArgumentOutOfRangeException InvalidItem(string type, object value)
            => new("", $"'{value}' is not a valid {type}");

        public static ArgumentOutOfRangeException NoItemFound(string type, object id)
            => new("", $"No {type} found with id '{id}'");

        public static InvalidOperationException RegisterActivePlayer(BsPlayer player)
            => new($"Player '{player.Id}' is already active and cannot be registered again");

        public static InvalidOperationException UnregisterInactivePlayer(BsPlayer player)
            => new($"Player '{player.Id}' is already inactive and cannot be unregistered again");

        public static ArgumentNullException BuildNullMap()
            => new("", "Cannot rebuild the map when no map is currently loaded");

        public static ArgumentOutOfRangeException OversizedMap(Vector2 size, float limit)
            => new("", $"Map size {size} exceeds the limit of {limit}");

        public static NullReferenceException MissingSubController(string masterTag, string requireeId, string requirementId)
            => new($"Subcontroller '{requireeId}' of object '{masterTag}' requires '{requirementId}'");

        public static ArgumentNullException ParentObjectUnbuilt()
            => new("", "Cannot create a child object when the parent object is unbuilt");

        public static InvalidOperationException PortCountMismatch(int updateResultCount, int outputPortCount)
            => new($"Cannot apply logic node update of length {updateResultCount} to output ports of length {outputPortCount}");

        public static InvalidDataException EmptyLcsDocument()
            => new("Cannot parse an empty LCS document");

        public static InvalidOperationException ErrorWhile(string action, Exception ex)
            => new($"Error while {action}: {ex.Message}\n{ex.StackTrace}\n");

        public static InvalidDataException ErrorWhile(string action, object context, Exception ex)
            => new($"Error while {action} '{context}': {ex.Message}\n{ex.StackTrace}\n");
    }
}
