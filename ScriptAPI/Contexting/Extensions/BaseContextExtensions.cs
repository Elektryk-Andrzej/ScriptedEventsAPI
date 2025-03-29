using System;
using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;

public static class BaseContextExtensions
{
    public static IEnumerator<float> ExecuteBaseContext(this BaseContext context)
    {
        switch (context)
        {
            case StandardContext standardContext:
                standardContext.Execute();
                yield return 0;
                break;
            case YieldingContext yieldingContext:
                yield return Timing.WaitUntilDone(yieldingContext.Execute().Run());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context), context, "context is not std nor yld");
        }
    }
}