﻿using System;
using System.Collections.Generic;
using MEC;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Exceptions;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.ForLoop;

public class ForLoopContext(Script scr) : TreeContext
{
    private readonly ResultStacker _rs = new("The `for` loop cannot be created.");
    private PlayerVariableToken? _loopVariableToken = null;
    private bool _isInKeywordAssigned = false;
    private PlayerVariableToken? _loopCollectionToken = null;
    private PlayerVariable _loopCollectionVariable = null!;
    private bool _skipRemainingContexts = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (_loopVariableToken == null)
        {
            if (token is not PlayerVariableToken variableToken)
            {
                return TryAddTokenRes.Error(_rs.AddInternal(
                    $"Value '{token.RawRepresentation}' must be a player variable."));
            }
            
            _loopVariableToken = variableToken;
            return TryAddTokenRes.Continue();

        }

        if (!_isInKeywordAssigned)
        {
            _isInKeywordAssigned = true;
            return token.RawRepresentation == "in"
                ? TryAddTokenRes.Continue()
                : TryAddTokenRes.Error(_rs.AddInternal(
                    $"Expected keyword 'in', got'{token.RawRepresentation}' instead."));
        }

        if (_loopCollectionToken == null)
        {
            if (token is not PlayerVariableToken variableToken)
            {
                return TryAddTokenRes.Error(_rs.AddInternal(
                    $"Value '{token.RawRepresentation}' must be a player variable."));
            }
            
            _loopCollectionToken = variableToken;
            return TryAddTokenRes.Continue();
        }
        
        return TryAddTokenRes.Error(_rs.AddInternal(
            $"Unexpected value '{token.RawRepresentation}'."));
    }

    public override Result VerifyCurrentState()
    {
        return _loopCollectionToken != null && _loopVariableToken != null && _isInKeywordAssigned
            ? true
            : _rs.AddInternal(
                "Loop is missing required parts.");
    }
    
    public override IEnumerator<float> Execute()
    {
        if (!scr.TryGetPlayerVariable(_loopCollectionToken!.NameWithoutPrefix, out _loopCollectionVariable))
        {
            throw new InvalidVariableException(_loopCollectionToken);
        }

        if (scr.TryGetPlayerVariable(_loopVariableToken!.NameWithoutPrefix, out _))
        {
            throw new Exception(
                $"Variable '{_loopVariableToken.RawRepresentation}' already exists, " +
                $"loop cannot create a new variable under the same name.");
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var plr in _loopCollectionVariable.Players())
        {
            var loopVariable = new PlayerVariable
            {
                Name = _loopVariableToken.NameWithoutPrefix,
                Players = () => [plr]
            };
            
            scr.LocalPlayerVariables.Add(loopVariable);
            foreach (var child in Children)
            {
                yield return Timing.WaitUntilDone(child.ExecuteBaseContext().Run());

                if (!_skipRemainingContexts)
                {
                    continue;
                }
                
                _skipRemainingContexts = false;
                break;
            }
            
            scr.LocalPlayerVariables.Remove(loopVariable);
        }
    }

    protected override void OnReceivedControlMessage(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.ForLoopContinue)
        {
            _skipRemainingContexts = true;
            return;
        }
        
        ParentContext?.SendControlMessage(msg);
    }
}






