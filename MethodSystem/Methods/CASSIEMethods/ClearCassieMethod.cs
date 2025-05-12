using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.CASSIEMethods;

public class ClearCassieMethod : Method
{
    public override string Description => "Clears all CASSIE announcements, active or queued.";
    
    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        Cassie.Clear();
    }
}