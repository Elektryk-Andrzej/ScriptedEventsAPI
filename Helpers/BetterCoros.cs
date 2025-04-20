using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace ScriptedEventsAPI.Helpers;

public static class BetterCoros
{
    public static CoroutineHandle Run(this IEnumerator<float> coro, Action<Exception>? onException = null)
    {
        return Timing.RunCoroutine(Wrapper(coro, onException));
    }

    public static void Kill(this CoroutineHandle coro)
    {
        Timing.KillCoroutines(coro);
    }

    private static IEnumerator<float> Wrapper(IEnumerator<float> routine, Action<Exception>? onException = null)
    {
        while (true)
        {
            try
            {
                if (!routine.MoveNext()) break;
            }
            catch (Exception ex)
            {
                Log.Error($"Coroutine failed with {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                onException?.Invoke(ex);
                yield break;
            }

            yield return routine.Current;
        }
    }

    public static IEnumerator<float> SlowWaitUntilTrue(Func<bool> condition)
    {
        while (true)
        {
            if (condition())
                break;

            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;
            yield return Timing.WaitForOneFrame;
        }
    }
}