using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace ScriptedEventsAPI.Helpers;

public static class BetterCoros
{
    public static CoroutineHandle Run(this IEnumerator<float> coro)
    {
        return Timing.RunCoroutine(Wrapper(coro));
    }
    
    public static void Kill(this CoroutineHandle routine)
    {
        Timing.KillCoroutines(routine);
    }
    
    private static IEnumerator<float> Wrapper(IEnumerator<float> routine)
    {
        while (true)
        {
            try
            {
                if (!routine.MoveNext())
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Coroutine failed with {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
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