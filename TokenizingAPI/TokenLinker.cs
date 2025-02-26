using System;
using System.Collections.Generic;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.TokenizingAPI.Contexts;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI;

/// <summary>
/// Responsible for joining file tokens together into contexts for execution.
/// </summary>
public class TokenLinker(Script script)
{
    public static BaseContext? TokenToContext(BaseToken token) => token switch
    {
        ActionToken actionToken => new ActionContext(actionToken),
        _ => null
    };

    public void LinkAllTokens()
    {
        List<BaseContext> contexts = [];
        BaseContext? currentContext = null;
        
        foreach (var token in script.Tokens)
        {
            Console.WriteLine($"Linking token {token.Name}");
            if (currentContext is not null)
            {
                Console.WriteLine($"Adding token to context {currentContext.Name}");
                if (currentContext.TryAddToken(token))
                {
                    continue;
                }
                
                Console.WriteLine($"Context {currentContext.Name} ended");
                contexts.Add(currentContext);
                currentContext = null;
            }

            var newCtx = TokenToContext(token);
            if (newCtx is null)
            {
                // add error
                Console.WriteLine($"Could not link token {token.Name}");
                continue;
            }

            currentContext = newCtx;
        }

        if (currentContext is not null)
        {
            Console.WriteLine($"Context {currentContext.Name} ended");
            contexts.Add(currentContext);
        }
        
        script.Contexts = contexts;
    }
}