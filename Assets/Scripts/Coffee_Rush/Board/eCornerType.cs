using System;

namespace Coffee_Rush.Board
{
    [Flags]
    public enum eCornerType : byte
    {
        None = 0,
        TopLeft = 1 << 0,
        TopRight = 1 << 1,
        BottomRight = 1 << 2,
        BottomLeft = 1 << 3
    }
}