using System;

namespace ScriptedEventsAPI.MethodAPI.Exceptions;

public class MalformedConditionException(string msg) : SystemException(msg);