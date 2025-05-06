using System;

namespace ScriptedEventsAPI.MethodSystem.Exceptions;

public class DeveloperFuckupException(string msg) : SystemException(msg);