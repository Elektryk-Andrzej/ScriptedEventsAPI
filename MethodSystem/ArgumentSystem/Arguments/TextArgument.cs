using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class TextArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<string> GetConvertSolution(BaseToken token, Script scr)
    {
        var value = token is ParenthesesToken parentheses
            ? parentheses.ValueWithoutBraces
            : token.RawRepresentation;

        return VariableParser.IsVariableUsedInString(value, scr, out var getProcessedVariableValueFunc)
            ? new(() => new()
            {
                Result = true,
                Value = getProcessedVariableValueFunc()
            })
            : new(new ArgEvalRes<string>.ResInfo
            {
                Result = true,
                Value = value
            });
    }
}