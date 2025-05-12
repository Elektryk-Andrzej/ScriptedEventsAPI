using System.Collections.Generic;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class PlayersArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Player variable e.g. @all";
    
    public static ArgumentEvaluation<List<Player>> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not PlayerVariableToken playerVariableToken)
        {
            return new(
                $"Value '{token.RawRepresentation}' is not a player variable."
            );
        }

        return new(() => DynamicSolver(playerVariableToken, scr));
    }

    private static ArgumentEvaluation<List<Player>>.EvalRes DynamicSolver(PlayerVariableToken token, Script scr)
    {
        var res = scr.TryGetPlayerVariable(token.NameWithoutPrefix, out var variable);
        if (res.HasErrored())
            return new()
            {
                Result = res,
                Value = null!
            };

        return new()
        {
            Result = true,
            Value = variable.Players()
        };
    }
}