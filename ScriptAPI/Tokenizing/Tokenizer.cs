using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing;

public class Tokenizer(Script script)
{
    public void GetAllFileTokens()
    {
        List<ScriptLine> tokens = [];
        for (var index = 0; index < script.Content.Split('\n').Length; index++)
        {
            var line = script.Content.Split('\n')[index];
            tokens.Add(GetTokensFromLine(line, index + 1));
        }

        script.Tokens = tokens;
    }

    public ScriptLine GetTokensFromLine(string lineContent, int lineNum)
    {
        return GetTokensFromLine(lineContent.ToCharArray(), lineNum);
    }

    public ScriptLine GetTokensFromLine(char[] lineContent, int lineNum, BaseToken? initialLexer = null)
    {
        var currentToken = initialLexer;
        List<BaseToken> tokens = [];

        foreach (var currentChar in lineContent)
        {
            if (currentToken is null)
            {
                currentToken = GetLexer(currentChar);

                if (currentToken is not null)
                {
                    Logger.Debug($"Set new token lexer to: {currentToken}");
                    currentToken.AddChar(currentChar);
                }

                continue;
            }
            
            if (!currentToken.EndParsingOnChar(currentChar))
            {
                currentToken.AddChar(currentChar);
                continue;
            }
            
            if (!char.IsWhiteSpace(currentChar))
            {
                Log.Warn($"Expected whitespace after {currentToken}, got character '{currentChar}' instead; converting to unclassified value");
                currentToken = new UnclassifiedValueToken(script, currentToken.RawRepresentation);
            }

            AddToken();
        }

        if (currentToken is not null)
        {
            AddToken();
        }

        return new()
        {
            LineNumber = lineNum,
            Script = script,
            Tokens = tokens
        };

        void AddToken()
        {
            if (currentToken.IsValidSyntax().HasErrored(out var msg))
            {
                Log.Error($"Token {currentToken} has failed: '{msg}'; converting to unclassified value");
                currentToken = new UnclassifiedValueToken(script, currentToken.RawRepresentation);
            }

            Logger.Debug($"Token lexer {currentToken} has stopped");
            tokens.Add(currentToken);
            currentToken = null;
        }
    }

    // if C# allowed for static override this wouldve been unnecessary
    private BaseToken? GetLexer(char character)
    {
        // whitespaces are ignored
        if (char.IsWhiteSpace(character)) return null;

        switch (character)
        {
            case '#':
                return new CommentToken(script);
            case '!':
                return new FlagToken(script);
            case '@':
                return new PlayerVariableToken(script);
            case '{':
                return new LiteralVariableToken(script);
            case '(':
                return new ParenthesesToken(script);
        }

        if (char.IsUpper(character)) return new MethodToken(script);

        if (char.IsLower(character)) return new KeywordToken(script);

        return new UnclassifiedValueToken(script);
    }
}