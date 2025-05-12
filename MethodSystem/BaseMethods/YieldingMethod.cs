using System.Collections.Generic;

namespace SER.MethodSystem.BaseMethods;

/// <summary>
///     Represents a SER method that can stop the execution of a script using its <see cref="IEnumerable{Float}" />.
/// </summary>
public abstract class YieldingMethod : BaseMethod
{
    public abstract IEnumerator<float> Execute();
}