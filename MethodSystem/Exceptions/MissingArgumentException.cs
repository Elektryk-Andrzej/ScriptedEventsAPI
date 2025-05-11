using System;

namespace ScriptedEventsAPI.MethodSystem.Exceptions;

public class MissingArgumentException(string msg) : SystemException(msg);