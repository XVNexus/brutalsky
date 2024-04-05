using System.Linq;
using Unity.Mathematics;
using Utils.Ext;
using Utils.Object;

namespace Utils
{
    public static class BsUtils
    {
        public static int Layer2Order(ObjectLayer layer)
        {
            return layer switch
            {
                ObjectLayer.Background => -2,
                ObjectLayer.Foreground => 2,
                _ => 0
            };
        }

        public static float CalculateDamage(float impactForce)
        {
            return MathfExt.TMP(impactForce, 20f, .5f);
        }

        public static uint GenerateId(string title, string author)
        {
            var idSeed = author.Aggregate<char, uint>(1, (current, letter) => current * letter)
                         + title.Aggregate<char, uint>(0, (current, letter) => current + letter);
            return Random.CreateFromIndex(idSeed).NextUInt();
        }

        public static string GenerateFullId(string tag, string id)
        {
            return $"{tag}:{id}";
        }

        public static string[] SplitFullId(string fullId)
        {
            return fullId.Split(':');
        }
    }
}
