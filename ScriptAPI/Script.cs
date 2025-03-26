using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.ScriptAPI.Contexting;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    public required string Name { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<BaseToken> Tokens = [];
    public List<BaseContext> Contexts = [];
    public HashSet<LiteralVariable> LocalLiteralVariables = [];

    public void Execute()
    {
        InternalExecute().Run();
    }

    private IEnumerator<float> InternalExecute()
    {
        new Tokenizer(this).GetAllFileTokens();
        Console.WriteLine(string.Join("\n", Tokens.Select(t => $"[{t.Name} - {string.Join("", t.Representation)}]")));
        
        new Contexter(this).LinkAllTokens();
        Console.WriteLine(string.Join("\n", Contexts.Select(t => $"[{t.Name}]")));
        
        foreach (var context in Contexts)
        {
            switch (context)
            {
                case StandardContext standardContext:
                    standardContext.Execute();
                    break;
                case YieldingContext yieldingContext:
                    yield return Timing.WaitUntilDone(yieldingContext.Execute());
                    break;
            }
        }
    }
}