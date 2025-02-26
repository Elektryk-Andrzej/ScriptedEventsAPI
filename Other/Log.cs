using System;

namespace ScriptedEventsAPI.Other;

public static class Log
{
    public static void Debug<T>(T obj)
    {
        if (obj is IFormattable formattable)
            Console.WriteLine($"Debug: {formattable.ToString()}");
        else
            Console.WriteLine($"Debug: {obj?.ToString() ?? "Error"}");
    }
}