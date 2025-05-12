using System;
using System.Collections.Generic;
using System.Linq;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.Exceptions;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem.ContextSystem.Contexts.Loops;

public class ForLoopContext(Script scr) : TreeContext
{
    private readonly ResultStacker _rs = new("The `for` loop cannot be created.");
    private bool _isInKeywordAssigned = false;
    private PlayerVariableToken? _loopCollectionToken = null;
    private PlayerVariable _loopCollectionVariable = null!;
    private PlayerVariableToken? _loopVariableToken = null;
    private bool _skipRemainingContexts = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_loopVariableToken == null)
        {
            if (token is not PlayerVariableToken variableToken)
                return TryAddTokenRes.Error(_rs.Add(
                    $"Value '{token.RawRepresentation}' must be a player variable."));

            _loopVariableToken = variableToken;
            return TryAddTokenRes.Continue();
        }

        if (!_isInKeywordAssigned)
        {
            _isInKeywordAssigned = true;
            return token.RawRepresentation == "in"
                ? TryAddTokenRes.Continue()
                : TryAddTokenRes.Error(_rs.Add(
                    $"Expected keyword 'in', got '{token.RawRepresentation}' instead."));
        }

        if (_loopCollectionToken == null)
        {
            if (token is not PlayerVariableToken variableToken)
                return TryAddTokenRes.Error(_rs.Add(
                    $"Value '{token.RawRepresentation}' must be a player variable."));

            _loopCollectionToken = variableToken;
            return TryAddTokenRes.Continue();
        }

        return TryAddTokenRes.Error(_rs.Add(
            $"Unexpected value '{token.RawRepresentation}'."));
    }

    public override Result VerifyCurrentState()
    {
        return _loopCollectionToken != null && _loopVariableToken != null && _isInKeywordAssigned
            ? true
            : _rs.Add("Loop is missing required parts.");
    }

    public override IEnumerator<float> Execute()
    {
        if (!scr.TryGetPlayerVariable(_loopCollectionToken!.NameWithoutPrefix, out _loopCollectionVariable))
            throw new InvalidVariableException(_loopCollectionToken);

        if (scr.TryGetPlayerVariable(_loopVariableToken!.NameWithoutPrefix, out _))
            throw new Exception(
                $"Variable '{_loopVariableToken.RawRepresentation}' already exists, " +
                $"loop cannot create a new variable under the same name.");

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var plr in _loopCollectionVariable.Players())
        {
            var loopVariable = new PlayerVariable
            {
                Name = _loopVariableToken.NameWithoutPrefix,
                Players = () => [plr]
            };

            scr.AddLocalPlayerVariable(loopVariable);
            foreach (var child in Children.TakeWhile(_ => !IsTerminated))
            {
                switch (child)
                {
                    case StandardContext standardContext:
                        standardContext.Execute();
                        break;
                    case YieldingContext yieldingContext:
                        var coro = yieldingContext.Execute();
                        while (coro.MoveNext())
                        {
                            yield return coro.Current;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(child), child, "context is not std nor yld");
                }

                if (!_skipRemainingContexts) continue;

                _skipRemainingContexts = false;
                break;
            }

            scr.RemoveLocalPlayerVariable(loopVariable);
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.ForLoopContinue)
        {
            _skipRemainingContexts = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}