using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Extensions;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Control;

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