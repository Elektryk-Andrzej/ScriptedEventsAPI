using System;

namespace ScriptedEventsAPI.Helpers;

public static class TimeSpanExtensions
{
    public static float ToFloatSeconds(this TimeSpan value)
    {
        return (float)value.TotalSeconds;
    }
}