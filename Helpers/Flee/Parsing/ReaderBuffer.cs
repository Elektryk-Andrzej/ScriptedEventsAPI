using System;
using System.IO;

namespace ScriptedEventsAPI.Helpers.Flee.Parsing;

/**
 * A character buffer that automatically reads from an input source
 * stream when needed. This class keeps track of the current position
 * in the buffer and its line and column number in the original input
 * source. It allows unlimited look-ahead of characters in the input,
 * reading and buffering the required data internally. As the
 * position is advanced, the buffer content prior to the current
 * position is subject to removal to make space for reading new
 * content. A few characters before the current position are always
 * kept to enable boundary condition checks.
 */
internal class ReaderBuffer
{
    public const int BlockSize = 1024;
    private char[] _buffer = new char[BlockSize * 4];
    private TextReader _input;

    public ReaderBuffer(TextReader input)
    {
        _input = input;
    }

    public int Position { get; private set; } = 0;

    public int LineNumber { get; private set; } = 1;

    public int ColumnNumber { get; private set; } = 1;

    public int Length { get; private set; } = 0;

    public void Dispose()
    {
        _buffer = null;
        Position = 0;
        Length = 0;
        if (_input != null)
        {
            try
            {
                _input.Close();
            }
            catch (Exception)
            {
                // Do nothing
            }

            _input = null;
        }
    }

    public string Substring(int index, int length)
    {
        return new string(_buffer, index, length);
    }

    public override string ToString()
    {
        return new string(_buffer, 0, Length);
    }

    public int Peek(int offset)
    {
        var index = Position + offset;

        // Avoid most calls to EnsureBuffered(), since we are in a
        // performance hotspot here. This check is not exhaustive,
        // but only present here to speed things up.
        if (index >= Length)
        {
            EnsureBuffered(offset + 1);
            index = Position + offset;
        }

        return index >= Length ? -1 : _buffer[index];
    }

    public string Read(int offset)
    {
        EnsureBuffered(offset + 1);
        if (Position >= Length) return null;

        var count = Length - Position;
        if (count > offset) count = offset;
        UpdateLineColumnNumbers(count);
        var result = new string(_buffer, Position, count);
        Position += count;
        if (_input == null && Position >= Length) Dispose();
        return result;
    }

    private void UpdateLineColumnNumbers(int offset)
    {
        for (var i = 0; i < offset; i++)
            if (_buffer[Position + i] == '\n')
            {
                LineNumber++;
                ColumnNumber = 1;
            }
            else
            {
                ColumnNumber++;
            }
    }

    private void EnsureBuffered(int offset)
    {
        // Check for end of stream or already read characters
        if (_input == null || Position + offset < Length) return;

        // Remove (almost all) old characters from buffer
        if (Position > BlockSize)
        {
            Length -= Position - 16;
            Array.Copy(_buffer, Position - 16, _buffer, 0, Length);
            Position = 16;
        }

        // Calculate number of characters to read
        var size = Position + offset - Length + 1;
        if (size % BlockSize != 0) size = (1 + size / BlockSize) * BlockSize;
        EnsureCapacity(Length + size);

        // Read characters
        try
        {
            while (_input != null && size > 0)
            {
                var readSize = _input.Read(_buffer, Length, size);
                if (readSize > 0)
                {
                    Length += readSize;
                    size -= readSize;
                }
                else
                {
                    _input.Close();
                    _input = null;
                }
            }
        }
        catch (IOException e)
        {
            _input = null;
            throw e;
        }
    }

    private void EnsureCapacity(int size)
    {
        if (_buffer.Length >= size) return;
        if (size % BlockSize != 0) size = (1 + size / BlockSize) * BlockSize;
        Array.Resize(ref _buffer, size);
    }
}