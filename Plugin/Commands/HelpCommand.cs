using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandSystem;
using SER.MethodSystem;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using SER.VariableSystem;

namespace SER.Plugin.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class HelpCommand : ICommand
{
    public string Command => "serhelp";
    public string[] Aliases => [];
    public string Description => string.Empty;
    
    private static List<BaseMethod> AllMethods => MethodIndex.NameToMethodIndex.Values.ToList();
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count == 0)
        {
            response = GetOptionsList();
            return true;
        }

        var arg = arguments.First().ToLower();

        switch (arg)
        {
            case "methods":
                response = GetMethodList();
                return true;
            case "variables":
                response = GetVariableList();
                return true;
        }
        
        var method = MethodIndex.NameToMethodIndex.Values
            .FirstOrDefault(met => met.Name.ToLower() == arg);
        if (method is not null)
        {
            response = GetMethodHelp(method);
            return true;
        }

        response = $"There is no '{arguments.First()}' option!";
        return false;
    }

    private static string GetOptionsList()
    {
        return """
               Welcome to the help command of SER! 
               
               To get specific information for your script creation adventure:
               (1) find the desired option (like 'methods')
               (2) use this command, attaching the option after it (like 'serhelp methods')
               (3) enjoy!
               
               Here are all the available options:
               > methods
               > variables
               > exiledevents
               """;
    }

    private static string GetMethodList()
    {
        var maxMethodLen = AllMethods.Max(m => m.Name.Length);
        
        Dictionary<string, List<BaseMethod>> methodsByCategory = new();
        foreach (var method in AllMethods)
        {
            if (methodsByCategory.ContainsKey(method.Subgroup))
            {
                methodsByCategory[method.Subgroup].Add(method);
            }
            else
            {
                methodsByCategory.Add(method.Subgroup, [method]);
            }
        }
        
        var sb = new StringBuilder($"Hi! There are {AllMethods.Count} methods available for your use!\n");
        sb.AppendLine("If a method has [txt], [plr] or [... ref], it represents a method returning text, players or an object reference accordingly.");
        
        foreach (var kvp in methodsByCategory)
        {
            sb.AppendLine();
            sb.AppendLine($"--- {kvp.Key} methods ---");
            foreach (var method in kvp.Value)
            {
                var name = method.Name;
                switch (method)
                {
                    case TextReturningMethod:
                        name += " [txt]";
                        break;
                    case PlayerReturningMethod:
                        name += " [plr]";
                        break;
                    case ReferenceReturningMethod refMethod:
                        name += $" [{refMethod.ReturnType.Name.ToLower()} ref]";
                        break;
                }

                if (maxMethodLen - name.Length > 0)
                {
                    var descFormatted = method.Description.Insert(0, new string(' ', maxMethodLen - name.Length));
                    sb.AppendLine($"> {name} {descFormatted}");
                }
                else
                {
                    sb.AppendLine($"> {name} {method.Description}");
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine("If you want to get specific information about a given method, just do 'serhelp MethodName'!");
        
        return sb.ToString();
    }
    
    private static string GetVariableList()
    {
        var allVars = PlayerVariableIndex.GlobalPlayerVariables.ToList();
        var sb = new StringBuilder($"Hi! There are {allVars.Count} variables available for your use!\n");
        
        allVars.ForEach(var => sb.AppendLine($"> @{var.Name}"));
        
        return sb.ToString();
    }

    private static string GetMethodHelp(BaseMethod method)
    {
        var sb = new StringBuilder($"=== {method.Name} ===\n");
        sb.AppendLine($"> {method.Description}");
        if (method is IAdditionalDescription addDesc)
        {
            sb.AppendLine($"> {addDesc.AdditionalDescription}");
        }

        sb.AppendLine();
        
        switch (method)
        {
            case TextReturningMethod:
                sb.AppendLine("This method returns a text value, which can be saved or used directly.");
                break;
            case PlayerReturningMethod:
                sb.AppendLine("This method returns a player value, which can be saved or used directly.");
                break;
            case ReferenceReturningMethod refMethod:
                sb.AppendLine($"This method returns a reference to {refMethod.ReturnType.Name} object, which can be saved or used directly.\n" +
                              $"References represent an object which cannot be fully represented in text.\n" +
                              $"If you wish to use that reference further, find methods supporting references of this type.");
                break;
        } 
        
        sb.AppendLine();

        if (method.ExpectedArguments.Length == 0)
        {
            sb.AppendLine("This method does not expect any arguments.");
            return sb.ToString();
        }
        
        sb.AppendLine("This method expects the following arguments:\n");
        for (var index = 0; index < method.ExpectedArguments.Length; index++)
        {
            var argument = method.ExpectedArguments[index];
            sb.AppendLine($"({index + 1}) '{argument.Name}' argument");
            
            if (argument.Description is not null)
            {
                sb.AppendLine($" - Description: {argument.Description}");
            }

            if (argument is OptionsArgument optionsArgument)
            {
                sb.AppendLine(" - Expected options are:");
                foreach (var option in optionsArgument.Options)
                {
                    sb.AppendLine(string.IsNullOrEmpty(option.Description)
                        ? $"  - {option.Value}"
                        : $"  - {option.Value} ({option.Description})");
                }
            }
            else
            {
                sb.AppendLine($" - Expected value: {argument.OperatingValueDescription}");
            }
            
            if (argument.IsOptional)
            {
                sb.AppendLine(
                    $" - Argument is optional! Default value: {argument.DefaultValue ?? "null"}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }   
}











