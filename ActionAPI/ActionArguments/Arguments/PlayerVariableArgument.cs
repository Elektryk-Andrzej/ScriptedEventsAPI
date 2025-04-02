using System;
using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class PlayerVariableArgument(string name) : BaseActionArgument(name)
{
    public static ArgEvalRes<Func<List<Player>>> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return new(
                $"Player variable argument requires it's value to be of type {nameof(PlayerVariableToken)}, " +
                         $"but got provided with type {token.GetType().Name}."
            );
        }

        var res = scr.TryGetPlayerVariable(playerVariableToken.NameWithoutPrefix, out var variable);
        return new(res, variable.Players);
    }
}