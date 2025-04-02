namespace ScriptedEventsAPI.ActionAPI.ActionResponses;

public struct MissingArgumentResponse : IActionResponse
{
    public required string ArgName { get; init; }
}