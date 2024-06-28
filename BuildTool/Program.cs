
/*
Dictionary<string, string?> properties = new();
for (int i = 0; i < args.Length; i++)
{
    if (args[i].StartsWith('-'))
    {
        if (args.Length > i+1 && !args[i + 1].StartsWith('-'))
        {
            properties[args[i]] = args[++i];
            continue;
        }

        properties[args[i]] = null;
    }
}
*/
string outputPath = "../../../../WebCompositionApp";

if (args.Length > 0 && args[0] != "editor")
{
    Console.WriteLine($"Welcome to the WebCompose build-tool. Modify the code of your app in {Path.GetFullPath("../../../../Core/src.wcp")}.");
    Console.WriteLine("If you want to build a wwwroot for the web editor, run 'BuildTool.exe editor'");
    Console.WriteLine($"If you want to build a wwwroot for the app you developed in {Path.GetFullPath("../../../../Core/src.wcp")}, just run the buildtool without any command line arguments");
    Console.WriteLine($"In both cases the wwwroot will be generated at {Path.GetFullPath(outputPath)}");
    Console.WriteLine("This is a little messy, I am sorry but I am kind of too lazy to write another command line interface.");
    Console.WriteLine($"If you need a little more functionality, consider modifying the build tool code at {Path.GetFullPath("../../../Program.cs")}.");
    return;
}

Directory.CreateDirectory(outputPath);

// Insert the app source code into the template and copy it to wwwroot
string src = File.ReadAllText("../../../../Core/src.wcp");
string template = File.ReadAllText(args.Length > 0 && args[0] == "editor" ? "../../../../Core/base.html" : "../../../../Core/standaloneBase.html");
template = template.Replace("{{srcGoesHere}}", src);
File.WriteAllText(Path.Join(outputPath, "index.html"), template);

// Copy the runtime wasm lib to wwwroot
File.Copy("../../../../Runtime/bin/bootsharp/index.mjs", Path.Join(outputPath, "index.js"), true);
Console.WriteLine($"Successfully created wwwroot at {Path.GetFullPath(outputPath)}");