﻿using System.Collections.Generic;

namespace Core.Composables;

public class Column : BlockComposable
{
    public override string GenerateHtml()
    {
        return $"<div class=\"column composable container\" style=\"display:flex;flex-direction:column\">{ExecuteBlock()}</div>";
    }
}