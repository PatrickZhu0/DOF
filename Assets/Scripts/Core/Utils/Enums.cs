namespace GameDLL.Enums
{
    public enum BOOL : byte
    {
        FALSE,
        TRUE,
    }

    public static class EnumUtils
    {
        public static bool Value(this BOOL value)
        {
            return value == BOOL.TRUE;
        }

        public static BOOL ToBOOL(this bool value)
        {
            if (value)
                return BOOL.TRUE;
            return BOOL.FALSE;
        }
    }

}
