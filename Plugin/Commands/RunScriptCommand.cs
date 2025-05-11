using System;
using System.Linq;
using CommandSystem;
using ScriptedEventsAPI.ScriptSystem;

namespace ScriptedEventsAPI.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class RunScriptCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var name = arguments.FirstOrDefault();
        
        if (name is null)
        {
            response = "No script name provided.";
            return false;
        }

        if (Script.CreateByScriptName(name).HasErrored(out var err, out var script))
        {
            response = err;
            return false;
        }
        
        script.Execute();
        response = "Script is now running!";
        return true;
    }

    public string Command => "serrun";
    public string[] Aliases => [];
    public string Description => string.Empty;
}