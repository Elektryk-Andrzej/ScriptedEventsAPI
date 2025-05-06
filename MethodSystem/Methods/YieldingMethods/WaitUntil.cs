using System.Collections.Generic;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.YieldingMethods;

public class WaitUntil : YieldingMethod
{
    public override string Description => "Halts the execution of the script until the given condition is true.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new ConditionArgument("condition")
    ];

    public override IEnumerator<float> Execute()
    {
        var condFunc = Args.GetConditionFunc("condition");
        return BetterCoros.SlowWaitUntilTrue(condFunc);
    }
}