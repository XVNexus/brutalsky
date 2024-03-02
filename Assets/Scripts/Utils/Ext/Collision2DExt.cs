using System.Linq;
using UnityEngine;

namespace Utils.Ext
{
    public static class Collision2DExt
    {
        public const float Atan2Percent = 0.636619772368f;

        public static float DirectnessFactor(this Collision2D @this)
        {
            return Mathf.Atan2(@this.TotalNormalImpulse(), Mathf.Abs(@this.TotalTangentImpulse())) * Atan2Percent;
        }

        public static float TotalNormalImpulse(this Collision2D @this)
        {
            return @this.contacts.Sum(contact => contact.normalImpulse);
        }

        public static float TotalTangentImpulse(this Collision2D @this)
        {
            return @this.contacts.Sum(contact => contact.tangentImpulse);
        }

        public static float TotalImpulse(this Collision2D @this)
        {
            return @this.contacts.Sum(contact => contact.normalImpulse + contact.tangentImpulse);
        }
    }
}
