using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.VariableAPI.Variables;

public class AllPlayers : PlayerVariable
{
    public override required string Name { get; init; } = "all";
    public override required Func<List<Player>> Players { get; init; } = () => Player.List.ToList();
}