using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class AmountArgument(string name, int minValue) : BaseMethodArgument(name)
{
    public ArgEvalRes<int> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr,
            out var getProcessedVariableValueFunc)
            ? new(() => InternalConvert(getProcessedVariableValueFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private ArgEvalRes<int>.ResInfo InternalConvert(string value)
    {
        if (!int.TryParse(value, out var result))
            return new()
            {
                Result = Rs.Add($"Value '{value}' cannot be interpreted as an integer."),
                Value = 0
            };

        if (result < minValue)
            return new()
            {
                Result = Rs.Add($"Value {result} is lower than allowed minimum value {minValue}."),
                Value = 0
            };

        return new()
        {
            Result = true,
            Value = result
        };
    }
}