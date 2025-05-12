namespace SER.ScriptSystem.ContextSystem.Structures;

public struct TryAddTokenRes()
{
    public static TryAddTokenRes Continue()
    {
        return new TryAddTokenRes
        {
            ShouldContinueExecution = true,
            HasErrored = false
        };
    }

    public static TryAddTokenRes Error(string errorMessage)
    {
        return new TryAddTokenRes
        {
            HasErrored = true,
            ShouldContinueExecution = false,
            ErrorMessage = errorMessage
        };
    }

    public static TryAddTokenRes End()
    {
        return new TryAddTokenRes
        {
            ShouldContinueExecution = false,
            HasErrored = false
        };
    }

    public required bool ShouldContinueExecution;
    public required bool HasErrored;
    public string ErrorMessage = "<error message not provideded!>";
}