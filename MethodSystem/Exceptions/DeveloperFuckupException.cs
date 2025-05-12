using System;

namespace SER.MethodSystem.Exceptions;

public class DeveloperFuckupException : SystemException
{
    public DeveloperFuckupException()
    {
    }
    
    public DeveloperFuckupException(string msg) : base(msg)
    {
    }
}