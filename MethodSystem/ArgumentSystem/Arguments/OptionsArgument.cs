using System;
using System.Linq;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class OptionsArgument(string name, params Option[] options) : BaseMethodArgument(name)
{
    public ArgEvalRes<string> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var replacedVariablesFunc)
            ? new(() => InternalConvert(replacedVariablesFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private ArgEvalRes<string>.ResInfo InternalConvert(string value)
    {
        var option = options.FirstOrDefault(opt => opt.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
        if (option == null)
            return new()
            {
                Result = $"Value '{value}' does not match any of the following options: " +
                         $"{string.Join(", ", options.Select(o => o.Value))}",
                Value = null!
            };

        return new()
        {
            Result = true,
            Value = option.Value
        };
    }
}