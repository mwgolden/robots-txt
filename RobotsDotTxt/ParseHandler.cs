namespace RobotsDotTxt;
public interface ParseHandler {
    public bool AllowedByRobots(Stream stream);
    public void handleKeyValuePair(string key, string value, int lineNumber);
}