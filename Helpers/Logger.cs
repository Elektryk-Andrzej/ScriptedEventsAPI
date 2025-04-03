using System;

namespace ScriptedEventsAPI.Helpers;

public static class Logger
{
    public static void Debug<T>(T obj)
    {
        if (obj is IFormattable formattable)
            Exiled.API.Features.Log.Info(formattable.ToString());
        else
            Exiled.API.Features.Log.Info($"Debug: {obj?.ToString() ?? "Error"}");
    }
}