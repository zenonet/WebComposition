using System.Net;

string path = "../../../../WebCompositionApp";
Directory.CreateDirectory(path);
File.Copy("../../../../Generator/base.html", Path.Join(path, "index.html"), true);
string src = File.ReadAllText("../../../../Generator/src.wcp");
string b = File.ReadAllText("../../../../Generator/base.html");
b = b.Replace("{{srcGoesHere}}", src);
File.WriteAllText(Path.Join(path, "index.html"), b);
//File.Copy("../../../../Runtime/bin/bootsharp/", Path.Join(path, "bootsharp"), true);