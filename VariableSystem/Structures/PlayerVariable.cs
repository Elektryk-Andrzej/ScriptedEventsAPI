using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace ScriptedEventsAPI.VariableSystem.Structures;

public class PlayerVariable
{
    public virtual required string Name { get; init; }
    public virtual required Func<List<Player>> Players { get; init; }
}