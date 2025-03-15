using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.TokenizingAPI;
using ScriptedEventsAPI.TokenizingAPI.Contexts;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    public string Content { get; set; } = string.Empty;
    public List<BaseToken> Tokens = [];
    public List<BaseContext> Contexts = [];

    public void Execute()
    {
        InternalExecute().Run();
    }

    private IEnumerator<float> InternalExecute()
    {
        new Tokenizer(this).GetAllFileTokens();
        Console.WriteLine(string.Join("\n", Tokens.Select(t => $"[{t.Name} - {string.Join("", t.Representation)}]")));
        
        new TokenLinker(this).LinkAllTokens();
        Console.WriteLine(string.Join("\n", Contexts.Select(t => $"[{t.Name}]")));
        
        foreach (var context in Contexts)
        {
            Log.Debug($"Executing {context.Name}...");
            yield return Timing.WaitUntilDone(context.Execute());
        }
    }
}