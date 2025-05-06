using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.BaseContexts;
using ScriptedEventsAPI.ScriptSystem.ContextSystem.Contexts.Control;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.Structures;

namespace ScriptedEventsAPI.ScriptSystem.ContextSystem;

/// <summary>
/// Responsible for joining file tokens together into contexts for execution.
/// </summary>
public class Contexter(Script script)
{
    private readonly List<BaseContext> _contexts = [];
    private readonly List<TreeContext> _treeContexts = [];

    public TryGet<List<BaseContext>> LinkAllTokens(List<ScriptLine> lines)
    {
        var rs = new ResultStacker($"Script {script.Name} cannot compile.");

        foreach (var line in lines)
        {
            var res = HandleLine(line);
            if (res.Value is null && string.IsNullOrEmpty(res.ErrorMsg)) continue;

            if (res.HasErrored(out var lineError, out var context)) return rs.Add(lineError);

            if (TryAddResult(context!, line.LineNumber).HasErrored(out var addError)) return rs.Add(addError);
        }

        Logger.Debug($"Contexting script {script.Name} has ended");
        return _contexts;
    }

    private Result TryAddResult(BaseContext context, int lineNum)
    {
        var rs = new ResultStacker($"Invalid context {context} in line {lineNum}.");

        if (context is TerminationContext)
        {
            if (_treeContexts.Count == 0) return rs.Add("There is no statement to end with the `end` keyword!");

            _treeContexts.RemoveAt(_treeContexts.Count - 1);
            return true;
        }

        if (context.VerifyCurrentState().HasErrored(out var error)) return rs.Add(error);

        var currentTree = _treeContexts.LastOrDefault();
        if (currentTree is not null)
        {
            Logger.Debug($"Adding finished context {context} to tree context {currentTree}");
            currentTree.Children.Add(context);
            context.ParentContext = currentTree;
        }
        else
        {
            Logger.Debug($"Adding finished context {context} to main collection");
            _contexts.Add(context);
        }

        if (context is TreeContext treeContext) _treeContexts.Add(treeContext);

        Logger.Debug($"Line {lineNum} has been contexted to {context}");
        return true;
    }

    private static TryGet<BaseContext?> HandleLine(ScriptLine line)
    {
        Logger.Debug($"Handling line {line.LineNumber}:");
        var rs = new ResultStacker($"Line {line.LineNumber} cannot execute");

        var firstToken = line.Tokens.FirstOrDefault();
        if (firstToken == null)
        {
            Logger.Debug($"Line {line.LineNumber} is empty");
            return null as BaseContext;
        }

        if (firstToken is not BaseContextableToken contextable)
        {
            Logger.Warn($"Line {line.LineNumber} does not start with a contextable token");
            return null as BaseContext;
        }

        var res = contextable.TryGetResultingContext();
        if (res.HasErrored(out var contextError))
            return rs.Add(contextError);

        var context = res.Value!;

        foreach (var token in line.Tokens.Skip(1))
        {
            if (HandleCurrentContext(token, context, out var endLineContexting).HasErrored(out var errorMsg))
                return rs.Add(errorMsg);

            if (endLineContexting) break;
        }

        return context;
    }

    private static Result HandleCurrentContext(BaseToken token, BaseContext context, out bool endLineContexting)
    {
        var rs = new ResultStacker($"Cannot add token {token} to context {context}");
        Logger.Debug($"Handling token {token} in context {context}");

        var result = context.TryAddToken(token);
        if (result.HasErrored)
        {
            endLineContexting = true;
            return rs.Add(result.ErrorMessage);
        }

        if (result.ShouldContinueExecution)
        {
            endLineContexting = false;
            return true;
        }

        endLineContexting = true;
        return true;
    }
}