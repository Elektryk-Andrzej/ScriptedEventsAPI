using System.IO;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.Plugin.ScriptFlags;
using ScriptedEventsAPI.ScriptSystem;

namespace ScriptedEventsAPI.Plugin;

public static class FileSystem
{
    public static readonly string DirPath = Path.Combine(Exiled.API.Features.Paths.Configs, "SER");
    public static string[] RegisteredScriptPaths = [];

    public static void UpdateScriptPathCollection()
    {
        RegisteredScriptPaths = Directory.GetFiles(DirPath, "*.txt");
    }
    
    public static void Initalize()
    {
        if (!Directory.Exists(DirPath))
        {
            Directory.CreateDirectory(DirPath);
            return;
        }
        
        UpdateScriptPathCollection();
        
        foreach (var scriptPath in RegisteredScriptPaths)
        {
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            
            Log.Info($"Found script '{scriptName}', checking for flags...");

            var lines = Script.CreateByVerifiedPath(scriptPath).GetFlagLines();
            
            ScriptFlagHandler.RegisterScript(lines, scriptName);
        }
    }
    
    public static bool DoesScriptExist(string scriptName, out string path)
    {
        UpdateScriptPathCollection();
        
        path = RegisteredScriptPaths.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == scriptName) ?? "";
        return !string.IsNullOrEmpty(path);
    }
    
    public static bool DoesScriptExist(string path)
    {
        UpdateScriptPathCollection();
        
        return RegisteredScriptPaths.Any(p => p == path);
    }
}