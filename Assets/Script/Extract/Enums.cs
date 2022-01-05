namespace Extract1
{
    public enum ImgVersion
    {
        Other = 0x00,
        Ver1 = 0x01,
        Ver2 = 0x02,
        Ver4 = 0x04,
        Ver5 = 0x05,
        Ver6 = 0x06,
        Ver7 = 0x07,
        Ver8 = 0x08,
        Ver9 = 0x09
    }

    /// <summary>
    ///     色位
    /// </summary>
    public enum ColorBits
    {
        ARGB_1555 = 0x0e,
        ARGB_4444 = 0x0f,
        ARGB_8888 = 0x10,
        LINK = 0x11,
        DXT_1 = 0x12,
        DXT_3 = 0x13,
        DXT_5 = 0x14,
        UNKNOWN = 0x00
    }

    /// <summary>
    ///     压缩类型
    /// </summary>
    public enum CompressMode
    {
        ZLIB = 0x06,
        NONE = 0x05,
        DDS_ZLIB = 0x07,
        UNKNOWN = 0x01
    }

}