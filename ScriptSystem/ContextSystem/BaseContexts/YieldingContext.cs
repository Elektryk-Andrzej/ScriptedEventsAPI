using System.Collections.Generic;

namespace SER.ScriptSystem.ContextSystem.BaseContexts;

public abstract class YieldingContext : BaseContext
{
    public abstract IEnumerator<float> Execute();
}