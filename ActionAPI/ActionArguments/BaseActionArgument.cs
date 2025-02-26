using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public abstract class BaseActionArgument(string name)
{
    public string Name { get; private set; } = name;
    public abstract bool CanConvert(BaseToken token);
    public abstract void SetValueWith(BaseToken token);
}