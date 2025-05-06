using System;
using System.Linq;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.VariableDefinition;

public class PlayerVariableDefinitionContext(PlayerVariableToken varToken, Script scr) : StandardContext
{
    private bool _hasEqualsSignBeenVerified = false;
    private PlayerReturningStandardMethod? _method;
    private MethodContext? _methodContext;
    private PlayerVariable? _variable;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        // emulating method context
        if (_methodContext != null) return _methodContext.TryAddToken(token);

        if (!_hasEqualsSignBeenVerified)
        {
            if (token.RawRepresentation != "=")
            {
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed.");
            }

            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        if (token is MethodToken methodToken)
        {
            if (methodToken.TryGetResultingContext().HasErrored(out var err, out var context))
                return TryAddTokenRes.Error(err);

            _methodContext = (MethodContext)context!;

            if (_methodContext.Method is not PlayerReturningStandardMethod playerMethod)
                return TryAddTokenRes.Error(
                    $"Method {methodToken.Method.Name} does not return a value, " +
                    "so you cannot use it to define a value of a variable.");

            _method = playerMethod;
            return TryAddTokenRes.Continue();
        }

        _variable = new()
        {
            Name = varToken.NameWithoutPrefix,
            Players = () => []
        };

        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        var rs = new ResultStacker($"Variable '{varToken.RawRepresentation}' cannot be created.");

        if (varToken.NameWithoutPrefix.Any(c => !char.IsLetter(c)))
            return rs.Add("Variable name can only contain letters.");

        if (char.IsUpper(varToken.NameWithoutPrefix.First()))
            return rs.Add("The first character in the name must be lowercase.");

        return _variable is not null || _method is not null
            ? true
            : rs.Add("There is no value to be assigned.");
    }

    public override void Execute()
    {
        if (_method != null)
        {
            Logger.Debug($"Executing '{_method.Name}' to get value");
            _method.Execute();

            if (_method.PlayerReturn == null)
            {
                Lg.M();
                throw new Exception($"Method {_method.Name} hasnt returned a value.");
            }

            Lg.M();
            Logger.Debug($"method returned {_method.PlayerReturn.Length} players to set the value of the variable");

            _variable = new()
            {
                Name = varToken.NameWithoutPrefix,
                Players = () => _method.PlayerReturn.ToList()
            };
        }
        else if (_variable is null)
        {
            Lg.M();
            Logger.Debug($"Tried to execute {GetType().Name} without a variable set.");
            throw new Exception($"Tried to execute {GetType().Name} without a variable set.");
        }

        Lg.M();
        Logger.Debug($"Added variable '{_variable.Name}' to script '{scr.Name}'.");
        scr.AddLocalPlayerVariable(_variable);
    }
}