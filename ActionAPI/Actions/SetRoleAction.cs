using PlayerRoles;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;
using ScriptedEventsAPI.ActionAPI.BaseActions;

namespace ScriptedEventsAPI.ActionAPI.Actions;

public class SetRoleAction : StandardAction
{
    public override string Name => "Setrole";
    public override string Description => "Sets a specified role for specified players.";

    public override BaseActionArgument[] ExpectedArguments { get; } =
    [
        new PlayerVariableArgument("players"),
        new EnumArgument("newRole", typeof(RoleTypeId))
    ];
    
    public override void Execute()
    {
        var players = Args.GetPlayers("players");
        var newRole = Args.GetEnum<RoleTypeId>("newRole");
        foreach (var player in players)
        {
            player.Role.Set(newRole);
        }
    }
}