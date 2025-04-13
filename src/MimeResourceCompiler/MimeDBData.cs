using System.Text;
using System.Text.Json;

namespace MimeResourceCompiler;

public interface IMimeDBData
{
    void GetData(List<Entry> list);
}

/// <summary>
/// ctor
/// </summary>
public sealed class MimeDBData(HttpClient client, ILogger log) : IMimeDBData
{
    private const string MIME_DB_URL = @"https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/db.json";
    private readonly HttpClient _httpClient = client;
    private readonly ILogger _log = log;

    public void GetData(List<Entry> list)
    {
        _log.Debug("Start connecting to mime-db data.");
        byte[] bytes = _httpClient.GetByteArrayAsync(MIME_DB_URL).Result;
        _log.Debug("Successfully connected to mime-db data.");

        var reader = new Utf8JsonReader(bytes);
        string mimeType = "";

        ReadOnlySpan<byte> extKey = "extensions"u8;

        int count = 0;

        while (reader.Read())
        {
            if (reader.CurrentDepth == 1)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    mimeType = Encoding.UTF8.GetString(reader.ValueSpan);
                }
            }
            else if (reader.CurrentDepth == 2)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                if (!extKey.SequenceEqual(reader.ValueSpan))
                {
                    continue;
                }

                reader.Read();

                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new FormatException("mime-DB probably changed the schema.");
                }

                while (true)
                {
                    reader.Read();

                    if (reader.TokenType != JsonTokenType.String)
                    {
                        break;
                    }

                    string? ext = reader.GetString();

                    if (ext != null)
                    {
                        list.Add(new Entry(mimeType, ext));
                        count++;
                    }
                }
            }
        }

        if (count == 0)
        {
            throw new FormatException("mime-DB probably changed the schema.");
        }

        _log.Information("{0} entries parsed from mime-db.", count);
    }
}