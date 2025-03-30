using System;
using System.Linq;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class TextArgument(string name) : BaseActionArgument(name)
{
    public static Result TryConvert(BaseToken token, Script scr, out Func<string> value)
    {
        switch (token)
        {
            case LiteralVariableToken literalVariableToken:
                value = () =>
                {
                    var variable = scr.LocalLiteralVariables.FirstOrDefault(v => v.Name == literalVariableToken.RawRepresentation);
                    return variable != null
                        ? variable.Value
                        : literalVariableToken.RawRepresentation;
                };
                return true;
            default:
                value = () => token.RawRepresentation;
                return true;
        }
    }
}