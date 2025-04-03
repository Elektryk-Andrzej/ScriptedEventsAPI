using System.Collections.Generic;

namespace ScriptedEventsAPI.MethodAPI.BaseMethods;

public abstract class YieldingMethod : BaseMethod
{
    public abstract IEnumerator<float> Execute();
}