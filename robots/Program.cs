using System;
using System.IO;
using System.Text;
using RobotsDotTxt;

string path = "../example-robots.txt";
FileStream fs = File.OpenRead(path);

Uri url = new Uri("http://www.contoso.com/title/index.htm");
string userAgent = "AgentName";
Matcher matcher = new Matcher(url, userAgent);
bool allowed = matcher.AllowedByRobots(fs);