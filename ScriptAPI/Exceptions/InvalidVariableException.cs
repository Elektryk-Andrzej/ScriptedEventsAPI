using System;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Exceptions;

public class InvalidVariableException : SystemException
{
    public InvalidVariableException(PlayerVariableToken token)
        : base($"Variable '{token.RawRepresentation}' does not exist!")
    {
    }
}