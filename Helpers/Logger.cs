using System;
using Discord;
using Exiled.API.Features;
using ScriptedEventsAPI.ScriptSystem;

namespace ScriptedEventsAPI.Helpers;

public static class Logger
{
    public static void Debug<T>(T obj)
    {
        //Log.Info($"Debug: {obj!.ToString()}");
    }

    public static void Warn<T>(T obj)
    {
        Log.Warn($"{obj!.ToString()}");
    }

    public static void Error<T>(Script scr, T obj)
    {
        Log.Send($"[Script '{scr.Name}'] {obj!.ToString()}", LogLevel.Error, ConsoleColor.Red);
    }
}