using System;
using Brutalsky;
using Brutalsky.Joint;
using UnityEngine;

namespace Utils
{
    public static class Errors
    {
        public static ArgumentException MapTooBig(Vector2 size, float limit)
            => new($"Map size {size} exceeds size limit of {limit}");

        public static ArgumentOutOfRangeException InvalidJointType(BsJointType jointType)
            => new(nameof(jointType), $"{jointType} is not a valid joint type");

        public static ArgumentOutOfRangeException MissingPathParams(string elementName, int requiredParams)
            => new(nameof(elementName), $"Cannot make a {elementName} with less than {requiredParams} parameters");

        public static ArgumentNullException NoTargetShape(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint without a target shape");

        public static ArgumentNullException TargetShapeUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint on an unbuilt shape");

        public static ArgumentNullException MountShapeUnbuilt(BsJoint joint)
            => new(nameof(joint), "Cannot create a joint attached to an unbuilt shape");
    }
}
