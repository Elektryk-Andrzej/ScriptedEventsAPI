using System;
using System.Collections.Generic;
using SER.Helpers;
using SER.ScriptSystem.ContextSystem.BaseContexts;

namespace SER.ScriptSystem.ContextSystem.Extensions;

public static class BaseContextExtensions
{
    public static IEnumerator<float> ExecuteBaseContext(this BaseContext context)
    {
        Logger.Debug($"Executing context {context.GetType().Name}");
        switch (context)
        {
            case StandardContext standardContext:
                standardContext.Execute();
                yield break;
            case YieldingContext yieldingContext:
                var coro = yieldingContext.Execute();
                while (coro.MoveNext())
                {
                    yield return coro.Current;
                }
                
                yield break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context), context, "context is not standard nor yielding");
        }
    }
}