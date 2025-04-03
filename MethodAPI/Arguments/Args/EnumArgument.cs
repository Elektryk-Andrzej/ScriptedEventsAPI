using System;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class EnumArgument(string name, Type enumType) : BaseMethodArgument(name)
{
    public ArgEvalRes<object> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var getProcessedVariableValueFunc)
            ? new(() => InternalConvert(getProcessedVariableValueFunc())) 
            : new(InternalConvert(token.RawRepresentation));
    }

    private ArgEvalRes<object>.ResInfo InternalConvert(string value)
    {
        if (!Enum.IsDefined(enumType, value))
        {
            return new()
            {
                Result = $"Enum {enumType.Name} does not include '{value}' as a valid value.",
                Value = null!
            };
        }

        var enumValue = Enum.Parse(enumType, value, true);
        return new()
        {
            Result = true,
            Value = enumValue
        };
    }
}