namespace Lexy.Compiler.Parser;

public class MarkdownLineFilter : ILineFilter
{
    private bool inCodeBlock;

    public bool UseLine(string line)
    {
        if (line.Trim() == "```")
        {
            inCodeBlock = !inCodeBlock;
            return false;
        }

        return inCodeBlock;
    }
}