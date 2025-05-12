using System.Collections.Generic;
using SER.Helpers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.YieldingMethods;

public class WaitUntilMethod : YieldingMethod
{
    public override string Description => "Halts execution of the script until the given condition is true.";

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