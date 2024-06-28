namespace Core;

public class LanguageException(string msg, int line) : Exception(msg)
{
    public int LineNumber = line;
}