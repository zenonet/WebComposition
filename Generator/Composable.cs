using System;
using System.Collections.Generic;
using Generator.Composables;

namespace Generator;

public abstract class Composable
{
    public static Dictionary<string, Type> ComposableDefinitions = new()
    {
        {"Text", typeof(Text)},
        {"Box", typeof(Box)},
        {"Column", typeof(Column)},
        {"Row", typeof(Row)},
        {"Button", typeof(Button)},
    };

    public abstract string GenerateHtml(List<Parameter> parameters, string? contentHtml = null);
}