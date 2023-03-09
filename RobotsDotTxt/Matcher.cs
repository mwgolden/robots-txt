namespace RobotsDotTxt;
using System.Diagnostics;
using System.Text;
public class Matcher : ParseHandler {
    public Uri Url {get; }
    public string UserAgent { get; }
    private bool userAgentMatch = false;
    private bool isUniversalAgent = false;
    private const string UNIVERSAL_AGENT = "*";

    public Matcher(Uri url, string userAgent){
        Url = url;
        UserAgent = userAgent;
    }
    public void handleKeyValuePair(string key, string value, int lineNumber){
        if(IsUserAgent(key)){
            handleUserAgent(value);
        }
        else if(userAgentMatch || isUniversalAgent){
            Debug.WriteLine($"Checking: {Url.AbsolutePath}");
            Debug.WriteLine($"[{lineNumber}]  {key}: {value}");
        }
    }
    public bool IsUserAgent(string key){
        return key == "User-agent";
    }
    public void handleUserAgent(string value){
        if(value.Trim() != UserAgent && value.Trim() != UNIVERSAL_AGENT){
            userAgentMatch = isUniversalAgent = false;
            return;
        }
        if(value.Trim() == UserAgent){
            userAgentMatch = true;
        }
        if(value.Trim() == UNIVERSAL_AGENT){
            isUniversalAgent = true;
        }
    }

    public bool AllowedByRobots(Stream stream){
        byte[] b = new byte[2038];
        using(ParseRobotsDotTxt rdt = new ParseRobotsDotTxt(stream, this)){
    
            ASCIIEncoding str = new ASCIIEncoding();
            int readLen;
            while((readLen = rdt.Read(b, 0, b.Length)) > 0){
                continue;
            }
        }
        return false;
    }
}