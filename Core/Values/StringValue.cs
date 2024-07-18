using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Core;

public class StringValue : Value
{
    public override string TypeName => "string";

    public string Value
    {
        get => (string)O!;
        set => O = value;
    }

    public static StringValue ConvertToString(Value val, int lineNumberForErrorMsg)
    {
        if (val is StringValue sv) return sv;
        if (val is IntValue iv) return new() {Value = iv.Value.ToString()};
        if (val is BoolValue bv) return new() {Value = bv.Value.ToString()};
        throw new LanguageException($"Can't implicitly convert {val.TypeName} to string.", lineNumberForErrorMsg);
    }
    public static bool TryConvertToString(Value val, [NotNullWhen(true)]out StringValue? stringValue)
    {
        if (val is IntValue iv) val =  new StringValue {Value = iv.Value.ToString()};
        if (val is BoolValue bv) val = new StringValue {Value = bv.Value.ToString()};

        if (val is not StringValue sv)
        {
            stringValue = null;
            return false;
        }

        stringValue = sv;
        return true;
    }
}