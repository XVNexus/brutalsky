using System.Linq;
using UnityEngine;

namespace Utils.Ext
{
    public static class Collision2DExt
    {
        public static float DirectnessFactor(this Collision2D _)
        {
            var normal = _.GetContact(0).normal;
            var velocity = _.relativeVelocity;
            var deltaAngle = Mathf.DeltaAngle(
                MathfExt.Atan2(normal) * Mathf.Rad2Deg,
                MathfExt.Atan2(velocity) * Mathf.Rad2Deg);
            return Mathf.Clamp01((90f - Mathf.Abs(deltaAngle)) / 90f);
        }

        public static float TotalNormalImpulse(this Collision2D _)
        {
            return _.contacts.Sum(contact => contact.normalImpulse);
        }

        public static float TotalTangentImpulse(this Collision2D _)
        {
            return _.contacts.Sum(contact => contact.tangentImpulse);
        }

        public static float TotalImpulse(this Collision2D _)
        {
            return _.contacts.Sum(contact => contact.normalImpulse + contact.tangentImpulse);
        }
    }
}
