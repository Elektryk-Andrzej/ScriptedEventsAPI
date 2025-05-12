using System.Collections.Generic;
using System.Linq;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.ScriptSystem.TokenSystem;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Control;

public class IfStatementContext(Script scr) : TreeContext
{
    private string? _condition;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition = token.GetValue();
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return _condition is not null
            ? true
            : "An if statement expects to have a condition, but none was provided!";
    }

    public override IEnumerator<float> Execute()
    {
        if (_condition is null)
        {
            yield break;
        }

        if (Condition.TryEval(_condition, scr).HasErrored(out var error, out var resul))
        {
            Logger.Error(scr, $"Error while evaluating condition: {error}");
            yield break;
        }
        
        if (resul == false)
        {
            yield break;
        }
        
        foreach (var child in Children.TakeWhile(_ => !IsTerminated))
        {
            var coro = child.ExecuteBaseContext();
            while (coro.MoveNext())
            {
                yield return coro.Current;
            }
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        ParentContext?.SendControlMessage(msg);
    }
}