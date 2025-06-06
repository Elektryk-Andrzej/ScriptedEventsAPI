﻿using LabApi.Features.Wrappers;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class IsCassieSpeakingMethod : TextReturningMethod, IPureMethod
{
    public override string Description => "Returns True/False value indicating if CASSIE is speaking.";

    public override BaseMethodArgument[] ExpectedArguments { get; } = [];
    
    public override void Execute()
    {
        TextReturn = Cassie.IsSpeaking.ToString();
    }
}