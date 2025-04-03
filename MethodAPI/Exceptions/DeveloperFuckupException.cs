using System;

namespace ScriptedEventsAPI.MethodAPI.Exceptions;

public class DeveloperFuckupException(string msg) : SystemException(msg);