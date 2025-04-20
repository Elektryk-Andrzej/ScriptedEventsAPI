using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Exceptions;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Exceptions;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts.Loops;

public class RepeatLoopContext(Script scr) : TreeContext
{
    private readonly ResultStacker _rs = new("Cannot create `repeat` loop.");
    private Func<string>? _getStringVal = null;
    private int? _repeatCount = null;
    private bool _skipChild = false;

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        if (VariableParser.IsVariableUsedInString(token.GetValue(), scr, out var resultFunc))
        {
            _getStringVal = resultFunc;
            return TryAddTokenRes.End();
        }

        if (!int.TryParse(token.GetValue(), out var resultInt))
            return TryAddTokenRes.Error(_rs.AddInt($"Value '{token.GetValue()}' is not a valid integer."));

        _repeatCount = resultInt;
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return Result.Assert(
            _getStringVal != null || _repeatCount.HasValue,
            _rs.AddInt("The amount of times to repeat was not provided."));
    }

    public override IEnumerator<float> Execute()
    {
        if (!_repeatCount.HasValue)
        {
            if (_getStringVal == null) throw new DeveloperFuckupException("Repeat context has no amount specified");

            var val = _getStringVal();
            if (!int.TryParse(val, out var resultInt)) throw new InvalidValueException("integer number", val);

            _repeatCount = resultInt;
        }

        for (var i = 0; i < _repeatCount.Value; i++)
        {
            if (IsTerminated)
            {
                yield break;
            }
            
            foreach (var child in Children.TakeWhile(_ => !IsTerminated))
            {
                yield return Timing.WaitUntilDone(child.ExecuteBaseContext().Run());

                if (!_skipChild) continue;

                _skipChild = false;
                break;
            }
        }
    }

    protected override void OnReceivedControlMessageFromChild(ParentContextControlMessage msg)
    {
        if (msg == ParentContextControlMessage.ForLoopContinue)
        {
            _skipChild = true;
            return;
        }

        ParentContext?.SendControlMessage(msg);
    }
}