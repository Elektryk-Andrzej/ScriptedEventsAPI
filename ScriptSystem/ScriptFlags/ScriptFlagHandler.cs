using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using Events = Exiled.Events.Handlers;

namespace SER.ScriptSystem.ScriptFlags;

public static class ScriptFlagHandler
{
    private record ScriptFlagInfo(string ScriptName, string[] FlagArguments);
    private static readonly Dictionary<SerScriptFlag, List<ScriptFlagInfo>> ScriptsWithSerFlags = [];
    private static readonly Dictionary<string, List<ScriptFlagInfo>> ScriptsWithCustomFlags = [];
    private static readonly HashSet<string> DynamicallyConnectedEvents = [];
    private static List<List<PropertyInfo>> _handlerTypes = [];
    private static List<Tuple<PropertyInfo, Delegate>> StoredDelegates { get; } = [];
    
    public static void Initialize()
    {
        Events.Server.RoundStarted += OnRoundStarted;
    }

    public static void ClearIndex()
    {
        ScriptsWithSerFlags.Clear();
        ScriptsWithCustomFlags.Clear();
    }
    
    public static void RegisterScript(List<ScriptLine> scriptLinesWithFlags, string scriptName)
    {
        foreach (var tokens in scriptLinesWithFlags.Select(sl => sl.Tokens.Skip(1).ToList()))
        {
            var flagName = tokens.First().GetValue();
            Log.Info($"Registering script '{scriptName}' with flag '{flagName}'");
            
            if (Enum.TryParse(flagName, true, out SerScriptFlag serFlag))
            {
                AddScriptToSerFlag(serFlag, scriptName, 
                    tokens.Skip(1).Select(t => t.GetValue()).ToArray());
            }
            else
            {
                AddScriptToCustomFlag(flagName, scriptName, 
                    tokens.Skip(1).Select(t => t.GetValue()).ToArray());
            }
        }
    }

    public static void RunScriptsWithCustomFlag(string flag)
    {
        foreach (var name in GetScriptNames(flag))
        {
            RunScriptInternal(name, flag);
        }
    }

    private static void RunScriptsWithFlag(SerScriptFlag flag, Func<string[], bool>? argPredicate = null)
    {
        foreach (var info in GetScriptNames(flag))
        {
            if (argPredicate is not null)
            {
                if (argPredicate(info.FlagArguments) == false) 
                    continue;
            }
            
            RunScriptInternal(info, flag.ToString());
        }
    }

    private static void RunScriptInternal(ScriptFlagInfo info, string flag)
    {
        if (Script.CreateByScriptName(info.ScriptName).HasErrored(out var err, out var script))
        {
            Log.Error($"Can't run script '{info.ScriptName}' with flag '{flag}'. {err}");
            return;
        }
            
        Log.Info($"Running script '{info.ScriptName}' with flag '{flag}'");
        script.Execute();
    }

    private static void AddScriptToCustomFlag(string flag, string scriptName, string[] flagArguments)
    {
        if (ScriptsWithCustomFlags.TryGetValue(flag, out var list))
        {
            list.Add(new(scriptName, flagArguments));
        }
        else
        {
            ScriptsWithCustomFlags.Add(flag, [new(scriptName, flagArguments)]);
        }
    }

    private static void AddScriptToSerFlag(SerScriptFlag flag, string scriptName, string[] flagArguments)
    {
        if (flag is SerScriptFlag.ExiledEvent)
        {
            if (!ConnectExiledEvent(flagArguments[0], scriptName))
            {
                Log.Error($"Event {flagArguments[0]} has failed to bind. Skipping script {scriptName} with flag {flag}.");
            }
            return;
        }
        
        if (ScriptsWithSerFlags.TryGetValue(flag, out var list))
        {
            list.Add(new(scriptName, flagArguments));
        }
        else
        {
            ScriptsWithSerFlags.Add(flag, [new(scriptName, flagArguments)]);
        }
    }

    private static List<ScriptFlagInfo> GetScriptNames(SerScriptFlag flag)
    {
        return ScriptsWithSerFlags.TryGetValue(flag, out var scriptNames) 
            ? scriptNames 
            : [];
    }

    private static List<ScriptFlagInfo> GetScriptNames(string customFlag)
    {
        return ScriptsWithCustomFlags.TryGetValue(customFlag, out var scriptNames) 
            ? scriptNames 
            : [];
    }
    
    private static bool ConnectExiledEvent(string eventName, string scriptName)
    {
        return ExiledEventSystem.TryBindScriptToExiledEvent(eventName, scriptName);
    }
    

    public static void OnRoundStarted()
    {
        RunScriptsWithFlag(SerScriptFlag.RoundStarted);
    }
}