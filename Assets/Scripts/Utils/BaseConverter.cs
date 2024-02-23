using UnityEngine;

namespace Utils
{
    public class BaseConverter
    {
        public string charset { get; }
        public int radix { get; }

        public BaseConverter(string charset)
        {
            this.charset = charset;
            radix = charset.Length;
        }

        public string Num2Base(int num)
        {
            var result = "";

            var quotient = num;
            while (quotient != 0)
            {
                var remainder = quotient % radix;
                quotient /= radix;
                result = charset[remainder] + result;
            }

            return num > 0 ? result : charset[0].ToString();
        }

        public int Base2Num(string baseNum)
        {
            var result = 0;

            for (var i = 0; i < baseNum.Length; i++)
            {
                var power = (int)Mathf.Pow(radix, i);
                var digit = baseNum[baseNum.Length - 1 - i];
                result += Symbol2Value(digit) * power;
            }

            return result;
        }

        public int Symbol2Value(char symbol)
        {
            return charset.IndexOf(symbol);
        }

        public char Value2Symbol(int value)
        {
            return value >= 0 && value < radix ? charset[value] : (char)0;
        }
    }
}
