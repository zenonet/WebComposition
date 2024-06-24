namespace Core;

public class VoidValue : Value
{
    public static readonly VoidValue I = new();
    public static readonly VoidValue Uninitialized = new();
}