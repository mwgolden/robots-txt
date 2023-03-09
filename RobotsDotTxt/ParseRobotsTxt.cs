namespace RobotsDotTxt;
using System.Diagnostics;
using System.Text;
public class ParseRobotsDotTxt : Stream {
    private readonly Stream stream;
    private Matcher matcher;
    private byte[] lineBuffer;
    private int lineNumber;
    private int linePosition;
    private const byte NEW_LINE = 0xA;
    private const byte CARRIAGE_RETURN = 0xD;
    private const byte SEPERATOR = 0x3A;
    private const byte COMMENT = 0x23;
    public ParseRobotsDotTxt(Stream stream, Matcher matcher) {
        if(stream == null) throw new ArgumentNullException("stream");
        this.stream = stream;
        this.matcher = matcher; 
        lineBuffer = new byte[2038 * 8];
        lineNumber = 0;
    }
    public override bool CanRead => stream.CanRead;
    public override bool CanSeek => stream.CanSeek;
    public override bool CanWrite => stream.CanWrite;
    public override long Length => stream.Length;
    public override long Position { 
        get => stream.Position; 
        set => stream.Position = value;
    }
    public override void Flush() => stream.Flush();
    public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);
    public override void SetLength(long value) => stream.SetLength(value);
    public override int Read(byte[] buffer, int offset, int count)
    {
        int numRead = stream.Read(buffer, offset, count);
        foreach(byte b in buffer){
            if(b != NEW_LINE && b !=  CARRIAGE_RETURN){
                if(linePosition < lineBuffer.Length){
                    lineBuffer[linePosition] = b;
                    linePosition++;
                }
            } else {
                
                parseLine(lineBuffer, linePosition);
                Array.Clear(lineBuffer);
                linePosition = 0;
            }
        }
        return numRead;
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }
    private void parseLine(byte[] buffer, int pos){
        byte[]? line = null;
        lineNumber++;
        int commentIndex = Array.IndexOf(buffer, COMMENT);
        //If commenIndex is zero, then the whole line is a comment
        if(commentIndex > 0){
            line = new byte[commentIndex];
            Array.Copy(buffer, line, commentIndex);
        }
        else if(commentIndex < 0){
            line = new byte[pos];
            Array.Copy(buffer, line, pos);
        }
        else {
            return;
        }
        if(HasKeyAndValuePair(line)){
            byte[] key = {};
            byte[] value = {};
            if(!getKeyAndValueFrom(line, ref key, ref value)){
                return;
            }
            matcher.handleKeyValuePair(Encoding.UTF8.GetString(key), Encoding.UTF8.GetString(value), lineNumber);
        }
    }
    private bool HasKeyAndValuePair(byte[] line){
        return Array.IndexOf(line, SEPERATOR) > -1;
    }
    private bool getKeyAndValueFrom(byte[] line, ref byte[] key, ref byte[] value){
        int index = Array.IndexOf(line, SEPERATOR);
        if(index < 0){
            return false;
        }
        key = new byte[index];
        value = new byte[line.Length - (index + 1)];
        Array.Copy(line, key, index);
        Array.Copy(line, index + 1, value, 0, value.Length);
        return true;
    }
}