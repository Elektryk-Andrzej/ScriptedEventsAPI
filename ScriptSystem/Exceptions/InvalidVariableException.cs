using System;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Tokens;

namespace ScriptedEventsAPI.ScriptSystem.Exceptions;

public class InvalidVariableException(PlayerVariableToken token)
    : SystemException($"Variable '{token.RawRepresentation}' does not exist!");