namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public abstract class BaseActionArgument(string name, bool required = true)
{
    public string Name { get; private set; } = name;
    public bool Required { get; private set; } = required;
}