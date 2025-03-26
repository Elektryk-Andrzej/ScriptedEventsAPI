using System;
using ScriptedEventsAPI.ActionAPI.BaseActions;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class LiteralVariableDefinitionContext(LiteralVariableToken varToken, Script scr) : StandardContext
{
    private LiteralVariable? _variable;
    private ActionContext? _actionContext;
    private StringReturningStandardAction? _action;
    private bool _hasEqualsSignBeenVerified = false;
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_actionContext != null)
        {
            return _actionContext.TryAddToken(token);
        }
        
        if (!_hasEqualsSignBeenVerified)
        {
            if (token.AsString != "=")
            {
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed. " +
                    "Example: {test} = Save (Hello, World!)");
            }
            
            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        if (token is ActionToken actionToken)
        {
            if (actionToken.Action is not StringReturningStandardAction resultStandardAction)
            {
                return TryAddTokenRes.Error(
                    "An action you are using does not return a value, " +
                    "so you cannot use it to define a value of a variable.");
            }

            _actionContext = new(actionToken, scr);
            _action = resultStandardAction;
            return TryAddTokenRes.Continue();
        }
        
        _variable = new(varToken, token.AsString);
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return new(
            _variable is not null,
            $"Cannot initalize a variable! There is no value to set the variable to.");
    }

    public override void Execute()
    {
        if (_action != null)
        {
            _action.Execute();

            if (_action.Result == null)
            {
                throw new Exception($"Tried to execute {GetType().Name}, but action result is null.");
            }
            
            _variable = new(varToken, _action.Result);
        }
        else if (_variable == null)
        {
            throw new Exception($"Tried to execute {GetType().Name} without a variable set.");
        }
        
        Logger.Debug($"Added variable '${_variable.Name}' to script '{scr.Name}'.");
        scr.LocalLiteralVariables.Add(_variable);
    }
}