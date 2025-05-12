using System;
using CommandSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class ReloadScriptsCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        FileSystem.Initalize();
        response = "Successfully reloaded scripts. Changes in script flags are now registered.";
        
        return true;
    }

    public string Command => "serreload";
    public string[] Aliases => [];
    public string Description => string.Empty;
}