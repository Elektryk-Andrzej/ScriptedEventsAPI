namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public abstract class BaseActionArgument(string name)
{
    public string Name { get; private set; } = name;
    public bool Required { get; init; } = true;
}