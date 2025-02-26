using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.TokenizingAPI;
using ScriptedEventsAPI.TokenizingAPI.Contexts;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    public string Content { get; set; } = string.Empty;
    public List<BaseToken> Tokens = [];
    public List<BaseContext> Contexts = [];

    public Exception? Execute()
    {
        try
        {
            new Tokenizer(this).GetAllFileTokens();
            Console.WriteLine(string.Join("\n", Tokens.Select(t => $"[{t.Name} - {string.Join("", t.Representation)}]")));
            
            new TokenLinker(this).LinkAllTokens();
            Console.WriteLine(string.Join("\n", Contexts.Select(t => $"[{t.Name}]")));
            
            foreach (var context in Contexts)
            {
                Console.WriteLine($"Executing {context.Name}...");
                context.Execute();
            }
        }
        catch (Exception e)
        {
            return e;
        }
        
        return null;
    }
    
}