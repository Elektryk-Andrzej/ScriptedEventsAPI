using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

public class PlayerVariableToken : BaseToken
{
    public string NameWithoutPrefix => RawRepresentation.Substring(1);
}