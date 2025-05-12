using System.Collections.Generic;
using System.Linq;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers.ResultStructure;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;

namespace SER.ScriptSystem.ContextSystem.Contexts.Loops;

public class ForeverLoopContext : TreeContext
{
    private readonly ResultStacker _rs = new("Cannot create `forever` loop.");
    private bool _skipChild = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Error(_rs.Add("Loop doesn't expect any arguments."));
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