using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class BoolArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<bool> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr,
            out var getProcessedVariableValueFunc)
            ? new(() => InternalConvert(getProcessedVariableValueFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private static ArgEvalRes<bool>.ResInfo InternalConvert(string value)
    {
        if (!bool.TryParse(value, out var result))
        {
            return new()
            {
                Result = $"Value '{value}' is not a valid true/false (boolean) value.",
                Value = false
            };
        }
        
        return new()
        {
            Result = true,
            Value = result
        };
    }
}