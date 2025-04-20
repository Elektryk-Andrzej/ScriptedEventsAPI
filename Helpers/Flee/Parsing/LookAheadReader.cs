using System;
using System.IO;
using System.Threading;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

// * A look-ahead character stream reader. This class provides the 
// * functionalities of a buffered line-number reader, but with the 
// * additional possibility of peeking an unlimited number of 
// * characters ahead. When looking further and further ahead in the 
// * character stream, the buffer is continously enlarged to contain 
// * all the required characters from the current position an 
// * onwards. This means that looking more characters ahead requires 
// * more memory, and thus becomes unviable in the end. 
internal class LookAheadReader : TextReader
{
    private const int StreamBlockSize = 4096;
    private const int BufferBlockSize = 1024;
    private char[] _buffer = new char[StreamBlockSize];
    private TextReader _input = null;
    private int _length;
    private int _pos;

    public LookAheadReader(TextReader input)
    {
        _input = input;
    }

    public int LineNumber { get; private set; } = 1;

    public int ColumnNumber { get; private set; } = 1;

    public override int Read()
    {
        ReadAhead(1);
        if (_pos >= _length) return -1;

        UpdateLineColumnNumbers(1);
        return Convert.ToInt32(_buffer[Math.Max(Interlocked.Increment(ref _pos), _pos - 1)]);
    }

    public override int Read(char[] cbuf, int off, int len)
    {
        ReadAhead(len);
        if (_pos >= _length) return -1;

        var count = _length - _pos;
        if (count > len) count = len;
        UpdateLineColumnNumbers(count);
        Array.Copy(_buffer, _pos, cbuf, off, count);
        _pos += count;
        return count;
    }

    public string ReadString(int len)
    {
        ReadAhead(len);
        if (_pos >= _length) return null;

        var count = _length - _pos;
        if (count > len) count = len;
        UpdateLineColumnNumbers(count);
        var result = new string(_buffer, _pos, count);
        _pos += count;
        return result;
    }

    public override int Peek()
    {
        return Peek(0);
    }

    public int Peek(int off)
    {
        ReadAhead(off + 1);
        if (_pos + off >= _length) return -1;

        return Convert.ToInt32(_buffer[_pos + off]);
    }

    public string PeekString(int off, int len)
    {
        ReadAhead(off + len + 1);
        if (_pos + off >= _length) return null;

        var count = _length - (_pos + off);
        if (count > len) count = len;
        return new string(_buffer, _pos + off, count);
    }

    public override void Close()
    {
        _buffer = null;
        _pos = 0;
        _length = 0;
        if (_input != null)
        {
            _input.Close();
            _input = null;
        }
    }

    private void ReadAhead(int offset)
    {
        var size = 0;
        var readSize = 0;

        // Check for end of stream or already read characters 
        if (_input == null || _pos + offset < _length) return;

        // Remove old characters from buffer 
        if (_pos > BufferBlockSize)
        {
            Array.Copy(_buffer, _pos, _buffer, 0, _length - _pos);
            _length -= _pos;
            _pos = 0;
        }

        // Calculate number of characters to read 
        size = _pos + offset - _length + 1;
        if (size % StreamBlockSize != 0)
        {
            size = size / StreamBlockSize * StreamBlockSize;
            size += StreamBlockSize;
        }

        EnsureBufferCapacity(_length + size);

        // Read characters 
        try
        {
            readSize = _input.Read(_buffer, _length, size);
        }
        catch (IOException e)
        {
            _input = null;
            throw;
        }

        // Append characters to buffer 
        if (readSize > 0) _length += readSize;
        if (readSize < size)
            try
            {
                _input.Close();
            }
            finally
            {
                _input = null;
            }
    }

    private void EnsureBufferCapacity(int size)
    {
        char[] newbuf = null;

        if (_buffer.Length >= size) return;
        if (size % BufferBlockSize != 0)
        {
            size = size / BufferBlockSize * BufferBlockSize;
            size += BufferBlockSize;
        }

        newbuf = new char[size];
        Array.Copy(_buffer, 0, newbuf, 0, _length);
        _buffer = newbuf;
    }

    private void UpdateLineColumnNumbers(int offset)
    {
        for (var i = 0; i <= offset - 1; i++)
            if (_buffer.Contains(_buffer[_pos + i]))
            {
                LineNumber += 1;
                ColumnNumber = 1;
            }
            else
            {
                ColumnNumber += 1;
            }
    }
}