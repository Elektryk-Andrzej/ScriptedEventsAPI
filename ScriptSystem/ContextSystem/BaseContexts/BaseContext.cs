using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Structures;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;

public abstract class BaseContext
{
    public string Name => GetType().Name;

    public TreeContext? ParentContext { get; set; } = null;

    public abstract TryAddTokenRes TryAddToken(BaseToken token);

    public override string ToString()
    {
        return Name;
    }

    public abstract Result VerifyCurrentState();

    protected virtual void Terminate(Script scr)
    {
        if (ParentContext != null)
        {
            ParentContext.Terminate(scr);
        }
        else
        {
            scr.Stop();
        }
    }
}