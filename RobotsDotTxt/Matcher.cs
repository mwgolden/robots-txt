namespace RobotsDotTxt;
using System.Diagnostics;
using System.Text;
public class Matcher : ParseHandler {
    public Uri Url {get; }
    public string UserAgent { get; }
    private bool userAgentMatch = false;
    private bool isUniversalAgent = false;
    private int score = 0;
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
            handleRule(key, value, lineNumber);
        }
    }
    public void handleRule(string key, string value, int lineNumber){
        bool isUrlMatch = Url.AbsoluteUri.Contains(value.Trim()) || value.Trim().Length == 0;
        if(isUrlMatch){
            Debug.WriteLine($"[{lineNumber}]  {key}: {value}");
            if(key.Trim().ToLower() == "disallow"){
                score -= value.Trim().Length;
            }
            if(key.Trim().ToLower() == "allow"){
                score += value.Trim().Length;
            }
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
        userAgentMatch = value.Trim() == UserAgent;
        isUniversalAgent = value.Trim() == UNIVERSAL_AGENT;
    }

    public bool AllowedByRobots(Stream stream){
        score = 0;
        byte[] b = new byte[2038];
        using(ParseRobotsDotTxt rdt = new ParseRobotsDotTxt(stream, this)){
    
            ASCIIEncoding str = new ASCIIEncoding();
            int readLen;
            while((readLen = rdt.Read(b, 0, b.Length)) > 0){
                continue;
            }
        }
        return score >= 0;
    }
}