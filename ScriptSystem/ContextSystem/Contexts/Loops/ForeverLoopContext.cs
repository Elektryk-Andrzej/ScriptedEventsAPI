using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Extensions;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Loops;

public class ForeverLoopContext : TreeContext
{
    private readonly ResultStacker _rs = new("Cannot create `forever` loop.");
    private bool _skipChild = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(_rs.Add($"Loop doesn't expect any arguments."));
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override IEnumerator<float> Execute()
    {
        while (true)
        {
            if (IsTerminated)
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

                if (!_skipChild) continue;

                _skipChild = false;
                break;
            }
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.ForLoopContinue)
        {
            _skipChild = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}