using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.TokenizingAPI.TokenLexers;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI;

public class Tokenizer(Script script)
{
    public void GetAllFileTokens()
    {
        List<BaseToken> tokens = [];
        foreach (var line in script.Content.Split('\n'))
        {
            tokens.AddRange(GetTokensFromLine(line));
        }
        
        script.Tokens = tokens;
    }

    public static List<BaseToken> GetTokensFromLine(string lineContent)
    {
        return InternalGetTokensFromFileLine(lineContent.ToCharArray().ToList());
    }
    
    public static List<BaseToken> GetTokensFromLine(List<char> lineContent)
    {
        return InternalGetTokensFromFileLine(lineContent);
    }

    public static List<BaseToken> InternalGetTokensFromFileLine(List<char> lineContent)
    {
        BaseTokenLexer? currentTokenLexer = default;
        List<BaseToken> tokens = [];
        bool isFirstChar = true;

        for (var index = 0; index < lineContent.Count; index++)
        {
            var character = lineContent[index];
            EaqoldHelpers.Nullable<char> nextChar = index + 1 < lineContent.Count 
                ? lineContent[index + 1] 
                : null;
            
            if (currentTokenLexer is not null)
            {
                currentTokenLexer.TryAddChar(character, out bool shouldContinueExecution); // should also return error
                if (shouldContinueExecution)
                {
                    continue;
                }

                if (nextChar.HasValue(out char c) && char.IsWhiteSpace(c))
                {
                    Console.Out.WriteLine("");
                    continue;
                }

                tokens.Add(currentTokenLexer.Token);
                currentTokenLexer = null;
            }

            // we're waiting for the first 
            if (isFirstChar)
            {
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                isFirstChar = false;
            }

            // whitespaces are ignored when no tokenizer is engaged
            if (char.IsWhiteSpace(character))
            {
                continue;
            }

            switch (character)
            {
                case '#':
                    return [new CommentToken(), new EndLineToken()];
                case '!':
                    currentTokenLexer = new FlagTokenLexer(character);
                    continue;
                case '@':
                    currentTokenLexer = new PlayerVariableTokenLexer(character);
                    continue;
                case '$':
                    currentTokenLexer = new LiteralVariableTokenLexer(character);
                    continue;
                case '(':
                    currentTokenLexer = new ParenthesesTokenLexer();
                    continue;
            }

            if (char.IsUpper(character))
            {
                currentTokenLexer = new ActionTokenLexer(character);
                continue;
            }

            if (char.IsLower(character) && isFirstChar)
            {
                currentTokenLexer = new ControlFlowTokenLexer(character);
                continue;
            }

            currentTokenLexer = new LiteralValueTokenLexer(character);
        }

        tokens.Add(new EndLineToken());
        return tokens;
    }
}