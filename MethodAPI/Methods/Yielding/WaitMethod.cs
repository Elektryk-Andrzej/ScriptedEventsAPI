using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.Yielding;

public class WaitMethod : YieldingMethod
{
    public override string Name => "Wait";
    public override string Description => "Waits for a specified amount of time.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new DurationArgument("duration")
    ];
    
    public override IEnumerator<float> Execute()
    {
        var dur = Args.GetDuration("duration");
        yield return Timing.WaitForSeconds((float)dur.TotalSeconds);
    }
}