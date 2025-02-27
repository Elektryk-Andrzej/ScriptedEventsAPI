using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments;

public class StringArgument(string name) : BaseActionArgument(name)
{
    public string Value { get; set; } = string.Empty;
    
    public static implicit operator string(StringArgument arg) => arg.Value;

    public override bool CanConvert(BaseToken token)
    {
        Log.Debug($"CanConvert {token}? {token is not EndLineToken}");
        return token is not EndLineToken;
    }

    public override void SetValueWith(BaseToken token)
    {
        Value = string.Join("", token.Representation);
    }
}