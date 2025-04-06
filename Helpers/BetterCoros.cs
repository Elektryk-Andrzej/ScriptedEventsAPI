using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace ScriptedEventsAPI.Helpers;

public static class BetterCoros
{
    public delegate bool ExceptionHandler(Exception ex);
    
    public static CoroutineHandle Run(this IEnumerator<float> coro, ExceptionHandler? exceptionHandler = null)
    {
        Logger.Debug($"is handler null? (1) {exceptionHandler == null}");
        return Timing.RunCoroutine(Wrapper(coro, exceptionHandler));
    }
    
    public static void Kill(this CoroutineHandle routine)
    {
        Timing.KillCoroutines(routine);
    }
    
    private static IEnumerator<float> Wrapper(IEnumerator<float> routine, ExceptionHandler? exceptionHandler = null)
    {
        Logger.Debug($"is handler null? (2) {exceptionHandler == null}");
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
                Logger.Debug($"is handler null? (3) {exceptionHandler == null}");
                Logger.Debug($"result? {exceptionHandler?.Invoke(ex) ?? false}");
                
                if (exceptionHandler == null || !exceptionHandler(ex))
                {
                    Log.Error($"Coroutine failed with {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                    yield break;
                }
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