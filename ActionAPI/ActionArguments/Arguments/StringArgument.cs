using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class StringArgument(string name) : BaseActionArgument(name)
{
    public string Value { get; set; } = string.Empty;
    
    public static implicit operator string(StringArgument arg) => arg.Value;
    
}