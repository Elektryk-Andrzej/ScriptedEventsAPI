using System;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.Exceptions;

public class InvalidVariableException(PlayerVariableToken token)
    : SystemException($"Variable '{token.RawRepresentation}' does not exist!");