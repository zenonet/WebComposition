using System.Diagnostics.CodeAnalysis;

namespace Core;

public static class ImplicitConversions
{
    public static StringValue AsString(this Value val, int lineNumberForErrorMsg)
    {
        if (val is StringValue sv) return sv;
        if (val is IntValue iv) return new() {Value = iv.Value.ToString()};
        if (val is BoolValue bv) return new() {Value = bv.Value.ToString()};
        throw new LanguageException($"Can't implicitly convert {val.TypeName} to string.", lineNumberForErrorMsg);
    }

    public static bool TryConvertToString(this Value val, [NotNullWhen(true)]out StringValue? stringValue)
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

    public static BoolValue AsBool(this Value val, int lineNumberForErrorMsg)
    {
        if (val is BoolValue bv) return bv;
        if (val is IntValue iv) return new() {Value = iv.Value != 0};
        throw new LanguageException($"Can't implicitly convert {val.TypeName} to bool.", lineNumberForErrorMsg);
    }
}