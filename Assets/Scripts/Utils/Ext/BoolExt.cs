namespace Utils.Ext
{
    public static class BoolExt
    {
        public static bool Parse(string raw)
            => raw == "1";

        public static string ToString(bool value)
            => value ? "1" : "0";
    }
}
