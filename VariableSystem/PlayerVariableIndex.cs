using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using PlayerRoles;
using SER.VariableSystem.Structures;

namespace SER.VariableSystem;

public static class PlayerVariableIndex
{
    public static readonly HashSet<PlayerVariable> GlobalPlayerVariables = [];

    public static void Initalize()
    {
        Clear();
        var allApiVariables = Enum
            .GetValues(typeof(RoleTypeId))
            .Cast<RoleTypeId>()
            .Select(roleType => {
                return new PlayerVariable
                {
                    Name = roleType.ToString().First().ToString().ToLower() + roleType.ToString().Substring(1) +
                           "Players",
                    Players = () => Player.Get(plr => plr.Role.Type == roleType).ToList(),
                };
            })
            .ToList();
        
        allApiVariables.AddRange(
            Enum.GetValues(typeof(Team))
                .Cast<Team>()
                .Select(teamType =>
                {
                    string name = teamType.ToString();
                    if (teamType is Team.SCPs)
                    {
                        name = "scp";
                    }
                    else if (name.EndsWith("s"))
                    {
                        name = name.Substring(0, name.Length - 1);
                    }

                    name = name.First().ToString().ToLower() + name.Substring(1) + "Players";
                    if (allApiVariables.Any(v => v.Name == name))
                    {
                        return null;
                    }
                    
                    return new PlayerVariable
                    {
                        Name = name,
                        Players = () => Player.Get(plr => plr.Role.Team == teamType).ToList(),
                    };
                })
                .OfType<PlayerVariable>());
        
        allApiVariables.Add(
            new PlayerVariable
            {
                Name = "allPlayers",
                Players = () => Player.List.ToList()
            });

        foreach (var variable in allApiVariables.OfType<PlayerVariable>())
        {
            AddPlayerVariable(variable);
        }
    }

    public static void AddPlayerVariable(PlayerVariable variable)
    {
        GlobalPlayerVariables.Add(variable);
    }

    public static void Clear()
    {
        GlobalPlayerVariables.Clear();
    }
}