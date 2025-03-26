using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.VariableAPI.Structures;

public class LiteralVariable(LiteralVariableToken token, string value)
{
    public string Name { get; private set; } = token.AsString;
    public string Value { get; private set; } = value;
}