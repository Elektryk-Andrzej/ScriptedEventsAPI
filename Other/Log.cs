using System;

namespace ScriptedEventsAPI.Other;

public static class Log
{
    public static void Debug<T>(T obj)
    {
        if (obj is IFormattable formattable)
            Exiled.API.Features.Log.Info(formattable.ToString());
        else
            Exiled.API.Features.Log.Info($"Debug: {obj?.ToString() ?? "Error"}");
    }
}