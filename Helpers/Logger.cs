using System;
using Exiled.API.Features;

namespace ScriptedEventsAPI.Helpers;

public static class Logger
{
    public static void Debug<T>(T obj)
    {
        if (obj is IFormattable formattable)
            Log.Info(formattable.ToString());
        else
            Log.Info($"Debug: {obj?.ToString() ?? "Error"}");
    }
    
    public static void Warn<T>(T obj)
    {
        if (obj is IFormattable formattable)
            Log.Warn(formattable.ToString());
        else
            Log.Warn($"Debug: {obj?.ToString() ?? "Error"}");
    }
}