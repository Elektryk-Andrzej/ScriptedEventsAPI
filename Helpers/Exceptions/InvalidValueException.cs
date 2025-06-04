﻿using System;

namespace SER.Helpers.Exceptions;

public class InvalidValueException(string expectedValue, string valueReceived)
    : SystemException(
        $"Context expected value '{expectedValue}', but received '{valueReceived}', which doesn't match.");