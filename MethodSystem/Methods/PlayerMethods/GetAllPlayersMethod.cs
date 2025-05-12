using System.Linq;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;

namespace SER.MethodSystem.Methods.PlayerMethods;

public class GetAllPlayersMethod : PlayerReturningMethod
{
    public override string Description => "Returns all players in the server.";

    public override BaseMethodArgument[] ExpectedArguments => [];
    
    public override void Execute()
    {
        PlayerReturn = Player.List.ToArray();
    }
}