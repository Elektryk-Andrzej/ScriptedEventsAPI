using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class NumberArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<float> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var replacedVariablesFunc)
            ? new(() => InternalConvert(replacedVariablesFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private static ArgEvalRes<float>.ResInfo InternalConvert(string value)
    {
        return float.TryParse(value, out var result)
            ? result
            : $"Value '{value}' is not a number.";
    }
}