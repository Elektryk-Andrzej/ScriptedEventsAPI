using Exiled.API.Features;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.MapMethods.WarheadMethods;

public class DetonateWarheadMethod : Method
{
    public override string Description => "Detonates the alpha warhead.";
    
    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        Warhead.Detonate();
    }
}