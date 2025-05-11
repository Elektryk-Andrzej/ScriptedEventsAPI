using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.YieldingMethods;

public class WaitMethod : YieldingMethod
{
    public override string Description => "Halts execution of the script for a specified amount of time.";

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