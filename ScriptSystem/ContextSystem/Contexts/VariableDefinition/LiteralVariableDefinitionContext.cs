using System;
using System.Linq;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;
using ScriptedEventsAPI.VariableSystem;
using ScriptedEventsAPI.VariableSystem.Structures;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.VariableDefinition;

public class LiteralVariableDefinitionContext(LiteralVariableToken varToken, Script scr) : StandardContext
{
    private bool _hasEqualsSignBeenVerified = false;
    private TextReturningMethod? _textReturningMethod;
    private ReferenceReturningMethod? _referenceReturningMethod;
    private MethodContext? _methodContext;
    private LiteralVariable? _variable;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        // emulating method context
        if (_methodContext != null) return _methodContext.TryAddToken(token);

        if (!_hasEqualsSignBeenVerified)
        {
            if (token.RawRepresentation != "=")
                return TryAddTokenRes.Error(
                    "When a line starts with a variable, the only possibility is setting said variable to a value, " +
                    "which requires a `=` sign after said variable, which is missing/malformed. " +
                    "Example: {test} = Save (Hello, World!)");

            _hasEqualsSignBeenVerified = true;
            return TryAddTokenRes.Continue();
        }

        if (token is not MethodToken methodToken)
        {
            _variable = new()
            {
                Name = varToken.NameWithoutBraces,
                Value = () => token.RawRepresentation
            };

            return TryAddTokenRes.End();
        }
        
        if (methodToken.TryGetResultingContext().HasErrored(out var err, out var context))
            return TryAddTokenRes.Error(err);

        _methodContext = (MethodContext)context!;

        switch (_methodContext.Method)
        {
            case TextReturningMethod textMethod:
                _textReturningMethod = textMethod;
                break;
            case ReferenceReturningMethod referenceMethod:
                _referenceReturningMethod = referenceMethod;
                break;
            default:
                return TryAddTokenRes.Error(
                    $"Method {methodToken.Method.Name} does not return a value, " +
                    "so you cannot use it to define a value of a variable.");
        }
            
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        var rs = new ResultStacker($"Variable '{varToken.RawRepresentation}' cannot be created.");

        if (varToken.NameWithoutBraces.FirstOrDefault() != '*')
        {
            if (_referenceReturningMethod is not null)
            {
                return rs.Add(
                    "When an action returns an object reference, " +
                    "'*' must be used before the name e.g. {*myVariable}");
            }
            
            if (varToken.NameWithoutBraces.Length == 0)
            {
                return rs.Add("Variable must have a name.");
            }
            
            if (varToken.NameWithoutBraces.Any(c => !char.IsLetter(c)))
                return rs.Add("Variable name can only contain letters.");

            if (char.IsUpper(varToken.NameWithoutBraces.First()))
                return rs.Add("The first character in the name must be lowercase.");
        }
        else
        {
            if (varToken.NameWithoutBraces.Length < 2)
            {
                return rs.Add("Variable must have a name, just '*' doesn't count as name.");
            }
        }

        return _variable is not null || _textReturningMethod is not null || _referenceReturningMethod is not null
            ? true
            : rs.Add("There is no value to be assigned.");
    }

    public override void Execute()
    {
        if (_textReturningMethod != null)
        {
            _textReturningMethod.Execute();

            if (_textReturningMethod.TextReturn == null)
            {
                Logger.Error(scr, $"Method {_textReturningMethod.Name} hasn't returned a value, variable {varToken.RawRepresentation} can't be created.");
                return;
            }
            
            _variable = new()
            {
                Name = varToken.NameWithoutBraces,
                Value = () => _textReturningMethod.TextReturn
            };
        }
        else if (_referenceReturningMethod != null)
        {
            _referenceReturningMethod.Execute();

            if (_referenceReturningMethod.ValueReturn == null)
            {
                Logger.Error(scr, $"Method {_referenceReturningMethod.Name} hasn't returned a value, variable {varToken.RawRepresentation} can't be created.");
                return;
            }

            _variable = new()
            {
                Name = varToken.NameWithoutBraces,
                Value = () => ObjectReferenceSystem.RegisterObject(_referenceReturningMethod.ValueReturn)
            };
        }
        else if (_variable is null)
        {
            Logger.Debug($"Tried to execute {GetType().Name} without a variable set.");
            throw new Exception($"Tried to execute {GetType().Name} without a variable set.");
        }

        Lg.M();
        Logger.Debug($"Added variable '{_variable.Name}' to script '{scr.Name}'.");
        scr.AddLocalLiteralVariable(_variable);
    }
}