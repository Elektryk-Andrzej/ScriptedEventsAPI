using System;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class TextArgument(string name, Func<string>? value = null) : BaseActionArgument(name)
{
    protected virtual string Value() => value?.Invoke() ?? "<default>";

    public override string ToString()
    {
        return Value();
    }
}