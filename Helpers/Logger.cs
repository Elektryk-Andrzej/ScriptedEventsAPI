using Exiled.API.Features;

namespace ScriptedEventsAPI.Helpers;

public static class Logger
{
    public static void Debug<T>(T obj)
    {
        Log.Info($"Debug: {obj!.ToString()}");
    }
    
    public static void Warn<T>(T obj)
    {
        Log.Warn($"Warn: {obj!.ToString()}");
    }
}