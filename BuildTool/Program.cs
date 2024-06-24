using System.Net;

string path = "../../../../WebCompositionApp";
Directory.CreateDirectory(path);
File.Copy("../../../../Core/base.html", Path.Join(path, "index.html"), true);
string src = File.ReadAllText("../../../../Core/src.wcp");
string b = File.ReadAllText("../../../../Core/base.html");
b = b.Replace("{{srcGoesHere}}", src);
File.WriteAllText(Path.Join(path, "index.html"), b);
//File.Copy("../../../../Runtime/bin/bootsharp/", Path.Join(path, "bootsharp"), true);