using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.VariableSystem.Structures;

namespace ScriptedEventsAPI.VariableSystem.Variables;

public class AllPlayers : PlayerVariable
{
    public override required string Name { get; init; } = "all";
    public override required Func<List<Player>> Players { get; init; } = () => Player.List.ToList();
}