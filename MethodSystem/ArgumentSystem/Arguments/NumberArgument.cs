using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

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