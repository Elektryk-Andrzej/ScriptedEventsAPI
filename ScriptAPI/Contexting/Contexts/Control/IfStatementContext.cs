using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Control;

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
            Logger.Error($"Error while evaluating condition: {error}");
            yield break;
        }
        
        Logger.Debug($"result of {_condition} was {resul}");
        if (resul == false)
        {
            yield break;
        }

        Logger.Debug($"if statement has children: {string.Join(", ", Children)}");
        foreach (var child in Children.TakeWhile(_ => !IsTerminated))
        {
            yield return Timing.WaitUntilDone(child.ExecuteBaseContext().Run());
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        ParentContext?.SendControlMessage(msg);
    }
}