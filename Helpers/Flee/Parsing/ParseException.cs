using System;
using System.Collections;
using System.Text;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A parse exception.
 */
public class ParseException : Exception
{
    public enum ErrorType
    {
        /**
         * The internal error type is only used to signal an error
         * that is a result of a bug in the parser or tokenizer
         * code.
         */
        INTERNAL,

        /**
         * The I/O error type is used for stream I/O errors.
         */
        IO,

        /**
         * The unexpected end of file error type is used when end
         * of file is encountered instead of a valid token.
         */
        UNEXPECTED_EOF,

        /**
         * The unexpected character error type is used when a
         * character is read that isn't handled by one of the
         * token patterns.
         */
        UNEXPECTED_CHAR,

        /**
         * The unexpected token error type is used when another
         * token than the expected one is encountered.
         */
        UNEXPECTED_TOKEN,

        /**
         * The invalid token error type is used when a token
         * pattern with an error message is matched. The
         * additional information provided should contain the
         * error message.
         */
        INVALID_TOKEN,

        /**
         * The analysis error type is used when an error is
         * encountered in the analysis. The additional information
         * provided should contain the error message.
         */
        ANALYSIS
    }

    private readonly ArrayList _details;


    /// <summary>
    ///     Creates a new parse exception.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="info"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    public ParseException(ErrorType type,
        string info,
        int line,
        int column)
        : this(type, info, null, line, column)
    {
    }

    /// <summary>
    ///     Creates a new parse exception. This constructor is only
    ///     used to supply the detailed information array, which is
    ///     only used for expected token errors. The list then contains
    ///     descriptions of the expected tokens.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="info"></param>
    /// <param name="details"></param>
    /// <param name="line"></param>
    /// <param name="column"></param>
    public ParseException(ErrorType type,
        string info,
        ArrayList details,
        int line,
        int column)
    {
        Type = type;
        Info = info;
        _details = details;
        Line = line;
        Column = column;
    }


    public ErrorType Type { get; }

    public string Info { get; }

    public ArrayList Details => new(_details);

    public int Line { get; }

    public int Column { get; }

    public override string Message
    {
        get
        {
            var buffer = new StringBuilder();

            // Add error description
            buffer.Append(ErrorMessage);

            // Add line and column
            if (Line > 0 && Column > 0)
            {
                buffer.Append(", on line: ");
                buffer.Append(Line);
                buffer.Append(" column: ");
                buffer.Append(Column);
            }

            return buffer.ToString();
        }
    }

    public string ErrorMessage
    {
        get
        {
            var buffer = new StringBuilder();

            // Add type and info
            switch (Type)
            {
                case ErrorType.IO:
                    buffer.Append("I/O error: ");
                    buffer.Append(Info);
                    break;
                case ErrorType.UNEXPECTED_EOF:
                    buffer.Append("unexpected end of file");
                    break;
                case ErrorType.UNEXPECTED_CHAR:
                    buffer.Append("unexpected character '");
                    buffer.Append(Info);
                    buffer.Append("'");
                    break;
                case ErrorType.UNEXPECTED_TOKEN:
                    buffer.Append("unexpected token ");
                    buffer.Append(Info);
                    if (_details != null)
                    {
                        buffer.Append(", expected ");
                        if (_details.Count > 1) buffer.Append("one of ");
                        buffer.Append(GetMessageDetails());
                    }

                    break;
                case ErrorType.INVALID_TOKEN:
                    buffer.Append(Info);
                    break;
                case ErrorType.ANALYSIS:
                    buffer.Append(Info);
                    break;
                default:
                    buffer.Append("internal error");
                    if (Info != null)
                    {
                        buffer.Append(": ");
                        buffer.Append(Info);
                    }

                    break;
            }

            return buffer.ToString();
        }
    }

    public ErrorType GetErrorType()
    {
        return Type;
    }

    public string GetInfo()
    {
        return Info;
    }

    public ArrayList GetDetails()
    {
        return Details;
    }

    public int GetLine()
    {
        return Line;
    }

    public int GetColumn()
    {
        return Column;
    }

    public string GetMessage()
    {
        return Message;
    }

    public string GetErrorMessage()
    {
        return ErrorMessage;
    }

    private string GetMessageDetails()
    {
        var buffer = new StringBuilder();

        for (var i = 0; i < _details.Count; i++)
        {
            if (i > 0)
            {
                buffer.Append(", ");
                if (i + 1 == _details.Count) buffer.Append("or ");
            }

            buffer.Append(_details[i]);
        }

        return buffer.ToString();
    }
}