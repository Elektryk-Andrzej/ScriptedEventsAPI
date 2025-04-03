namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public abstract class BaseMethodArgument(string name)
{
    public string Name { get; private set; } = name;
    public bool Required { get; init; } = true;
}