namespace MimeResourceCompiler.Classes;

public class Compressor(ILogger log) : ICompressor
{
    private readonly ILogger _log = log;

    public void RemoveUnreachableEntries(List<Entry> list)
    {
        int removedItemsCount = 0;

        for (int i = list.Count - 1; i >= 1; i--)
        {
            bool equalsMimeType = false;
            bool equalsExtension = false;
            Entry currentEntry = list[i];

            for (int j = i - 1; j >= 0; j--)
            {
                Entry comp = list[j];

                if (comp.MimeType.Equals(currentEntry.MimeType, StringComparison.Ordinal))
                {
                    equalsMimeType = true;
                }

                if (comp.Extension.Equals(currentEntry.Extension, StringComparison.Ordinal))
                {
                    equalsExtension = true;
                }

                if (equalsMimeType && equalsExtension)
                {
                    list.RemoveAt(i);
                    _log.Debug("  {0} removed.", currentEntry);
                    removedItemsCount++;
                    break;
                }
            }
        }

        _log.Information("{0} unreachable entries removed.", removedItemsCount);
    }
}
