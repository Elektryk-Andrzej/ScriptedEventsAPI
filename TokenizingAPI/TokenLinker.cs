using System;
using System.Collections.Generic;
using Exiled.API.Features;
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
            if (token is EndLineToken)
            {
                if (currentContext is not null)
                {
                    contexts.Add(currentContext);
                    currentContext = null;
                }
                continue;
            }
            
            if (currentContext is not null)
            {
                var result = currentContext.TryAddToken(token);
                if (result.HasErrored)
                {
                    Log.Error($"Error linking '{currentContext}': {result.Message}");
                    break;
                }

                if (result.ShouldContinueExecution)
                {
                    continue;
                }
                
                Log.Info($"Context {currentContext.Name} ended");
                contexts.Add(currentContext);
                currentContext = null;
                continue;
            }

            var newCtx = TokenToContext(token);
            if (newCtx is null)
            {
                // add error
                Log.Info($"Could not link token {token.Name}");
                continue;
            }

            currentContext = newCtx;
        }

        if (currentContext is not null)
        {
            Log.Info($"Context {currentContext.Name} ended");
            contexts.Add(currentContext);
        }
        
        script.Contexts = contexts;
    }
}