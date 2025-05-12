using System;

namespace SER.MethodSystem.Exceptions;

public class ArgumentFetchException(string msg) : SystemException(msg);