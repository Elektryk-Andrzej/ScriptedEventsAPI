using System;

namespace SER.MethodSystem.Exceptions;

public class MissingArgumentException(string msg) : SystemException(msg);