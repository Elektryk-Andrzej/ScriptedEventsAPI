using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

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