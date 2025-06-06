﻿using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SER.Helpers.Extensions;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods;

public class DestroyRagdollsMethod : Method
{
    public override string Description => "Destroys ragdolls.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new EnumArgument<RoleTypeId>("roleToRemove")
        {
            DefaultValue = RoleTypeId.None,
            Description = "Do not provide this argument to destroy all ragdolls."
        }
    ];
    
    public override void Execute()
    {
        var roleToRemove = Args.GetEnum<RoleTypeId>("roleToRemove");
        IEnumerable<Ragdoll> ragdolls = Ragdoll.List;
        if (roleToRemove != RoleTypeId.None)
        {
            ragdolls = ragdolls.Where(rd => rd.Role == roleToRemove);
        }
        
        ragdolls.ForEachItem(rd => rd.Destroy());
    }
}