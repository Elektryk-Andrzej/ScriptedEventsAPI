using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class TextArgument(string name) : BaseActionArgument(name)
{
    public static ArgEvalRes<string> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr, out var getProcessedVariableValueFunc)
            ? new(() => new()
            {
                Result = true,
                Value = getProcessedVariableValueFunc(),
            })
            : new(new ArgEvalRes<string>.ResInfo
            {
                Result = true,
                Value = token.RawRepresentation,
            });
    }
}