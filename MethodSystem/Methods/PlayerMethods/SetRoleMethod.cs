using PlayerRoles;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem.Methods.PlayerMethods;

public class SetRoleMethod : Method
{
    public override string Description => "Sets a role for players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayersArgument("players"),
        new EnumArgument<RoleTypeId>("newRole")
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var newRole = Args.GetEnum<RoleTypeId>("newRole");
        foreach (var player in players) player.Role.Set(newRole);
    }
}