namespace ScriptedEventsAPI.ActionAPI.ActionResponses;

public class MissingArgumentResponse(string argName) : IActionResponse
{
    public string ArgName { get; set; } = argName;
}