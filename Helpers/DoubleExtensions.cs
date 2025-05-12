using System;

namespace SER.Helpers;

public static class TimeSpanExtensions
{
    public static float ToFloatSeconds(this TimeSpan value)
    {
        return (float)value.TotalSeconds;
    }
}