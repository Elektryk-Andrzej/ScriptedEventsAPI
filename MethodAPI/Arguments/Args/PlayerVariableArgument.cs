using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public class PlayerVariableArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<List<Player>> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return new(
                $"Value '{token.RawRepresentation}' is not a player variable."
            );
        }

        return new(() => DynamicSolver(playerVariableToken, scr));
    }

    private static ArgEvalRes<List<Player>>.ResInfo DynamicSolver(PlayerVariableToken token, Script scr)
    {
        var res = scr.TryGetPlayerVariable(token.NameWithoutPrefix, out var variable);
        if (res.HasErrored())
        {
            return new()
            {
                Result = res,
                Value = null!
            };
        }
        
        return new()
        {
            Result = true,
            Value = variable.Players()
        };
    }
}