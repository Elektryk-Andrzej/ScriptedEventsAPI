using Exiled.API.Features;

namespace ScriptedEventsAPI.MethodSystem.BaseMethods;

/// <summary>
/// Represents a standard method that returns an array of players.
/// </summary>
public abstract class PlayerReturningStandardMethod : StandardMethod
{
    public Player[]? PlayerReturn { get; protected set; }
}