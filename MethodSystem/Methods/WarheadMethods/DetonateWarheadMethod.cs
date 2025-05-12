using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.MapMethods.WarheadMethods;

public class DetonateWarheadMethod : Method
{
    public override string Description => "Detonates the alpha warhead.";
    
    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        Warhead.Detonate();
    }
}