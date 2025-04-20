using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandSystem;
using ScriptedEventsAPI.ScriptAPI;

namespace ScriptedEventsAPI.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class RunScriptCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        new Script
        {
            Name = Path.GetFileNameWithoutExtension(arguments.First().Replace("\"", "")),
            Content = File.ReadAllText(arguments.First(), Encoding.UTF8)
        }.Execute();

        response = string.Empty;
        return true;
    }

    public string Command => "runscr";
    public string[] Aliases => [];
    public string Description => string.Empty;
}