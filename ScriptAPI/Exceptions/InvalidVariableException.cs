using System;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Exceptions;

public class InvalidVariableException(PlayerVariableToken token)
    : SystemException($"Variable '{token.RawRepresentation}' does not exist!");