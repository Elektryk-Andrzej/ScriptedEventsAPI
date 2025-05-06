using System;

namespace ScriptedEventsAPI.MethodSystem.Exceptions;

public class ArgumentFetchException(string msg) : SystemException(msg);