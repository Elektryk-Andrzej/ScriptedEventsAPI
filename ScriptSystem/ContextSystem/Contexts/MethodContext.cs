using System.Collections.Generic;
using System.Linq;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.MethodSystem.ArgumentSystem;
using SER.MethodSystem.BaseMethods;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.ContextSystem.Structures;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.ScriptSystem.TokenSystem.Tokens;

namespace SER.ScriptSystem.ContextSystem.Contexts;

public class MethodContext(MethodToken methodToken) : YieldingContext
{
    public readonly BaseMethod Method = methodToken.Method;
    public readonly MethodArgumentProcessor Processor = new(methodToken.Method, methodToken.Method.Script);

    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        Logger.Debug($"Checking if {token} is a valid argument for method {Method.Name}.");
        if (Processor.IsValidArgument(token, Method.Args.Count, out var skeleton).HasErrored(out var error))
            return TryAddTokenRes.Error(
                $"Value '{token.RawRepresentation}' is not a valid argument for method {Method.Name}, because: {error}");

        Logger.Debug(
            $"Adding argument '{skeleton.Name}' (index {Method.Args.Count}) (type {skeleton.ArgumentType.Name}) to method '{Method.Name}'.");
        Method.Args.Add(skeleton);
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        var requiredArgs = Method.ExpectedArguments.Count(arg => arg.DefaultValue is null);
        var providedArgs = Method.Args.Count;

        return providedArgs >= requiredArgs
            ? true
            : $"Method '{Method.Name}' is missing required arguments! " +
              $"Provided arguments: {providedArgs}, required arguments: {requiredArgs}";
    }

    public override IEnumerator<float> Execute()
    {
        Logger.Debug($"'{Method.Name}' method is now running..");

        switch (Method)
        {
            case Method stdAct:
                stdAct.Execute();
                yield break;
            case YieldingMethod yieldAct:
                var enumerator = yieldAct.Execute();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                yield break;
        }
    }
}