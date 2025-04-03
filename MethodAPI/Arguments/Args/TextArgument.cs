using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class TextArgument(string name) : BaseMethodArgument(name)
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