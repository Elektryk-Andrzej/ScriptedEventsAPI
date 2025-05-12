using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class TextArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Text value e.g. (Never Gonna Give You Up)";
    
    public static ArgumentEvaluation<string> GetConvertSolution(BaseToken token, Script scr)
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
            : new(new ArgumentEvaluation<string>.EvalRes
            {
                Result = true,
                Value = value
            });
    }
}