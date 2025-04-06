using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting;

/// <summary>
/// Responsible for joining file tokens together into contexts for execution.
/// </summary>
public class Contexter(Script script)
{
    private readonly List<BaseContext> _contexts = [];
    private BaseContext? _currentContext = null;
    private readonly List<TreeContext> _treeContexts = [];

    private void HandleCurrentContext(BaseToken token)
    {
        Logger.Debug($"Handling token {token} in context {_currentContext}");
        
        var result = _currentContext!.TryAddToken(token);
        if (result.HasErrored)
        {
            Log.Error($"Error linking '{_currentContext}': {result.ErrorMessage}");
            return;
        }

        if (result.ShouldContinueExecution)
        {
            return;
        }

        AddCurrentContext();
        _currentContext = null;
    }

    private void AddCurrentContext()
    {
        if (_currentContext is null)
        {
            return;
        }
        
        if (_currentContext is TerminationContext)
        {
            if (_treeContexts.Count == 0)
            {
                Log.Error("No context to end with the `end` keyword!");
                return;
            }
            
            _treeContexts.RemoveAt(_treeContexts.Count - 1);
            return;
        }

        if (_currentContext!.VerifyCurrentState().HasErrored(out var error))
        {
            Logger.Debug($"ERROR!: {error}");
        }
        
        var currentTree = _treeContexts.LastOrDefault();
        if (currentTree is not null)
        {
            Logger.Debug($"Adding finished context {_currentContext} to tree context {currentTree}");
            currentTree.Children.Add(_currentContext);
            _currentContext.ParentContext = currentTree;
        }
        else
        {
            Logger.Debug($"Adding finished context {_currentContext} to main collection");
            _contexts.Add(_currentContext);
        }
        
        if (_currentContext is TreeContext treeContext)
        {
            _treeContexts.Add(treeContext);
        }
    }

    public List<BaseContext> LinkAllTokens(List<BaseToken> tokens)
    {
        var isFirstContextOfLine = true;
        foreach (var token in tokens)
        {
            Logger.Debug($"Checking token {token}");
            if (token is EndLineToken)
            {
                isFirstContextOfLine = true;
                AddCurrentContext();
                _currentContext = null;
                continue;
            }
            
            if (_currentContext is not null)
            {
                HandleCurrentContext(token);
                continue;
            }

            if (!isFirstContextOfLine)
            {
                Logger.Debug($"Skipping token {token}, not first in line + context does not take it into account");
                continue;
            }

            if (token is not BaseContextableToken contextable)
            {
                Logger.Debug($"Starting token {token} is not contextable!");
                continue;
            }

            var newCtx = contextable.GetResultingContext();
            if (newCtx is null)
            {
                // add error
                Log.Error($"Could not link token {token.TokenName}");
                continue;
            }

            Logger.Debug($"Set new context to {newCtx}");
            _currentContext = newCtx;
        }

        if (_currentContext is null)
        {
            return _contexts;
        }
        
        Log.Info($"Context {_currentContext.Name} ended");
        _contexts.Add(_currentContext);
        return _contexts;
    }
}