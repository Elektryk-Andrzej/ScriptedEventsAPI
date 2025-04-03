using System;

namespace ScriptedEventsAPI.MethodAPI.Exceptions;

public class ArgumentFetchException(string msg) : SystemException(msg);