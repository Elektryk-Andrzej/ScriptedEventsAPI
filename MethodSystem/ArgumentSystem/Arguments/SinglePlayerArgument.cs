using System.Linq;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class SinglePlayerArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => "Player variable with a single player";
    
    public static ArgumentEvaluation<Player> GetConvertSolution(BaseToken token, Script scr)
    {
        if (token is not PlayerVariableToken playerVariableToken)
            return new(
                $"Value '{token.RawRepresentation}' is not a player variable."
            );

        return new(() => DynamicSolver(playerVariableToken, scr));
    }

    private static ArgumentEvaluation<Player>.EvalRes DynamicSolver(PlayerVariableToken token, Script scr)
    {
        var res = scr.TryGetPlayerVariable(token.NameWithoutPrefix, out var variable);
        if (res.HasErrored())
            return new()
            {
                Result = res,
                Value = null!
            };

        var plrs = variable.Players();
        if (plrs.Count != 1)
            return new()
            {
                Result = $"The player variable must have exactly 1 player, but has {plrs.Count} instead.",
                Value = null!
            };

        return new()
        {
            Result = true,
            Value = plrs.First()
        };
    }
}