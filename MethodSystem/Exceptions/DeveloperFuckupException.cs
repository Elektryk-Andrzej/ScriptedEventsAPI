using System;

namespace ScriptedEventsAPI.MethodSystem.Exceptions;

public class DeveloperFuckupException : SystemException
{
    public DeveloperFuckupException()
    {
    }
    
    public DeveloperFuckupException(string msg) : base(msg)
    {
    }
}