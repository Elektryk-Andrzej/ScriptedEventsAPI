using System;
using System.Collections.Generic;
using System.Linq;
using PluginAPI.Core;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Structures;
using Events = Exiled.Events.Handlers;

namespace ScriptedEventsAPI.Plugin.ScriptFlags;

public static class ScriptFlagHandler
{
    private static readonly Dictionary<SerScriptFlag, List<string>> ScriptsWithSerFlags = [];
    private static readonly Dictionary<string, List<string>> ScriptsWithCustomFlags = [];
    
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
        foreach (var flagName in scriptLinesWithFlags.Select(scriptLine => scriptLine.Tokens[1].GetValue()))
        {
            Log.Info($"Registering script '{scriptName}' with flag '{flagName}'");
            
            if (Enum.TryParse(flagName, true, out SerScriptFlag serFlag))
            {
                AddScriptToSerFlag(serFlag, scriptName);;
            }
            else
            {
                AddScriptToCustomFlag(flagName, scriptName);
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

    private static void RunScriptsWithFlag(SerScriptFlag flag)
    {
        foreach (var name in GetScriptNames(flag))
        {
            RunScriptInternal(name, flag.ToString());
        }
    }

    private static void RunScriptInternal(string name, string flag)
    {
        if (Script.CreateByScriptName(name).HasErrored(out var err, out var script))
        {
            Log.Error($"Can't run script '{name}' with flag '{flag}'. {err}");
            return;
        }
            
        Log.Info($"Running script '{name}' with flag '{flag}'");
        script.Execute();
    }

    private static void AddScriptToCustomFlag(string flag, string scriptName)
    {
        if (ScriptsWithCustomFlags.TryGetValue(flag, out var list))
        {
            list.Add(scriptName);
        }
        else
        {
            ScriptsWithCustomFlags.Add(flag, [scriptName]);
        }
    }

    private static void AddScriptToSerFlag(SerScriptFlag flag, string scriptName)
    {
        if (ScriptsWithSerFlags.TryGetValue(flag, out var list))
        {
            list.Add(scriptName);
        }
        else
        {
            ScriptsWithSerFlags.Add(flag, [scriptName]);
        }
    }

    private static List<string> GetScriptNames(SerScriptFlag flag)
    {
        return ScriptsWithSerFlags.TryGetValue(flag, out var scriptNames) 
            ? scriptNames 
            : [];
    }

    private static List<string> GetScriptNames(string customFlag)
    {
        return ScriptsWithCustomFlags.TryGetValue(customFlag, out var scriptNames) 
            ? scriptNames 
            : [];
    }

    public static void OnRoundStarted()
    {
        RunScriptsWithFlag(SerScriptFlag.RoundStarted);
    }
}