using System;
using System.Collections.Generic;
using System.Reflection;
using Exiled.API.Features;
using MEC;

namespace ScriptedEventsAPI.EaqoldHelpers;

public static class SafeCoroutines
{
    public static CoroutineHandle Run(this IEnumerator<float> routine)
    {
        return Timing.RunCoroutine(RunCoroutineWrapper(routine));
    }
    
    public static void Kill(this CoroutineHandle routine)
    {
        Timing.KillCoroutines(routine);
    }
    
    private static IEnumerator<float> RunCoroutineWrapper(IEnumerator<float> routine)
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
                Log.Error($"Coroutine failed from plugin '{Assembly.GetCallingAssembly().GetName().Name}':\n{ex.Message}\t\n{ex.StackTrace}");
                yield break;
            }
            
            yield return routine.Current;
        }
    }
}