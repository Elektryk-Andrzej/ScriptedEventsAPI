using System;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class LiteralVariableDefinitionContext(LiteralVariableToken varToken, Script scr) : StandardContext
{
    private LiteralVariable? _variable;
    private LineMethodContext? _actionContext;
    private TextReturningStandardMethod? _action;
    private bool _hasEqualsSignBeenVerified = false;
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_actionContext != null)
        {
            return _actionContext.TryAddToken(token);
        }
        
        if (!_hasEqualsSignBeenVerified)
        {
            if (token is not VariableDefinitionToken)
            {
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed. " +
                    "Example: {test} = Save (Hello, World!)");
            }
            
            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        if (token is MethodToken actionToken)
        {
            if (actionToken.Method is not TextReturningStandardMethod resultStandardAction)
            {
                return TryAddTokenRes.Error(
                    "An method you are using does not return a value, " +
                    "so you cannot use it to define a value of a variable.");
            }

            _actionContext = new(actionToken, scr);
            _action = resultStandardAction;
            return TryAddTokenRes.Continue();
        }
        
        _variable = new()
        {
            Name = varToken.NameWithoutBraces,
            Value = token.RawRepresentation,
        };
        
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return _variable is not null || _action is not null
            ? true 
            :  "Cannot initalize a variable! There is no value to set the variable to.";
    }

    public override void Execute()
    {
        if (_action != null)
        {
            Logger.Debug($"Executing '{_action.Name}' to get value");
            _action.Execute();

            if (_action.TextReturn == null)
            {
                Lg.M();
                throw new Exception($"Tried to execute {GetType().Name}, but method result is null.");
            }
            
            Lg.M();
            Logger.Debug($"method returned {_action.TextReturn} to set the value of the variable");

            _variable = new()
            {
                Name = varToken.NameWithoutBraces,
                Value = _action.TextReturn
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
        scr.LocalLiteralVariables.Add(_variable);
    }
}