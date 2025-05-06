using System;

namespace ScriptedEventsAPI.MethodSystem.Exceptions;

public class MalformedConditionException(string msg) : SystemException(msg);