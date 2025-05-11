using Exiled.API.Features;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.CASSIEMethods;

public class ClearCassieMethod : Method
{
    public override string Description => "Clears all CASSIE announcements, active or queued.";
    
    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        Cassie.Clear();
    }
}