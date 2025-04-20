using PlayerRoles;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class SetRoleMethod : StandardMethod
{
    public override string Name => "Setrole";
    public override string Description => "Sets a specified role for specified players.";

    public override BaseMethodArgument[] ExpectedArguments { get; } =
    [
        new PlayerVariableArgument("players"),
        new EnumArgument("newRole", typeof(RoleTypeId))
    ];

    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var newRole = Args.GetEnum<RoleTypeId>("newRole");
        foreach (var player in players) player.Role.Set(newRole);
    }
}