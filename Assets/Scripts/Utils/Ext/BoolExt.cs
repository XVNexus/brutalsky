namespace Utils.Ext
{
    public static class BoolExt
    {
        public static bool Parse(string raw)
        {
            return raw == "1";
        }

        public static string Stringify(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
