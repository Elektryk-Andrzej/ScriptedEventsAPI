using System.Diagnostics;
using System.IO;
using Exiled.API.Features;

namespace ScriptedEventsAPI.OtherStructures;
public static class Lg
{
    /// <summary>
    /// Debug
    /// </summary>
    /// <param name="str"></param>
    public static void D(string str)
    {
        Log.Debug($"{GetTrace()}\n<color=yellow>|\t {str}</color>");
    }
    
    /// <summary>
    /// Info
    /// </summary>
    /// <param name="str"></param>
    public static void I(string str)
    {
        Log.Info($"{GetTrace()}\n<color=yellow>|\t {str}</color>");
    }
    
    /// <summary>
    /// Error
    /// </summary>
    /// <param name="str"></param>
    public static void E(string str)
    {
        Log.Error($"{GetTrace()}\n<color=yellow>|\t {str}</color>");
    }
    
    /// <summary>
    /// Mark
    /// </summary>
    public static void M(string str = "")
    {
        Log.Info($"{GetTrace()}" + (string.IsNullOrEmpty(str) ? "" : $" -> {str}"));
    }
    
    /// <summary>
    /// Mark
    /// </summary>
    public static void Dm(string str = "")
    {
        Log.Debug($"{GetTrace()}" + (string.IsNullOrEmpty(str) ? "" : $" -> {str}"));
    }

    private static string GetTrace()
    {
        var stackFrame = new StackFrame(2, true); // Get the caller info
        var method = stackFrame.GetMethod();
        var fileName = Path.GetFileName(stackFrame.GetFileName());
        var lineNumber = stackFrame.GetFileLineNumber(); // Line number
        return $"({fileName} | {method?.Name ?? "unknown"} | {lineNumber})";
    }
}