using System;
using System.Text.RegularExpressions;
using Utils.Ext;
using Utils.Object;

namespace Utils
{
    public static class BsUtils
    {
        public static string RepeatChar(char character, int repeats)
        {
            var result = "";
            for (var i = 0; i < repeats; i++)
            {
                result += character;
            }
            return result;
        }

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
            return BitConverter.ToUInt32(BitConverter.GetBytes((title + author).GetHashCode()), 0);
        }

        public static string CleanId(string id)
        {
            return Regex.Replace(id.Replace(' ', '-').ToLower(), "[^a-z0-9-]|-(?=-)", "");
        }
    }
}
