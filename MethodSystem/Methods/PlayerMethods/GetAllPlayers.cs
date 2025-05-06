using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class GetAllPlayers : PlayerReturningStandardMethod
{
    public override string Description => "Returns all players in the server.";

    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        PlayerReturn = Player.List.ToArray();
    }
}