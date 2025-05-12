using System;
using CommandSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class StopAllCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = $"Stopped {Plugin.RunningScripts.Count} scripts.";
            
        foreach (var script in Plugin.RunningScripts)
        {
            script.Stop();
        }
        
        return true;
    }

    public string Command => "serstopall";
    public string[] Aliases => [];
    public string Description => string.Empty;
}