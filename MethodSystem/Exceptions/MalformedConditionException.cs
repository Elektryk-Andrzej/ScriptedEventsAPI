using System;

namespace SER.MethodSystem.Exceptions;

public class MalformedConditionException(string msg) : SystemException(msg);