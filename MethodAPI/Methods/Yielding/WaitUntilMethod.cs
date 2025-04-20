using System.Collections.Generic;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.Yielding;

public class WaitUntilMethod : YieldingMethod
{
    public override string Name => "WaitUntil";
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