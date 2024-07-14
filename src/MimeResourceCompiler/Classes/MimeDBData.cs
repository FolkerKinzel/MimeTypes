using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace MimeResourceCompiler.Classes;

public sealed class MimeDBData : IMimeDBData
{
    private const string MIME_DB_URL = @"https://cdn.jsdelivr.net/gh/jshttp/mime-db@master/db.json";
    private readonly HttpClient _httpClient;
    private readonly ILogger _log;

    /// <summary>
    /// ctor
    /// </summary>
    public MimeDBData(HttpClient client, ILogger log)
    {
        _httpClient = client;
        _log = log;
    }

    public List<Entry> GetData()
    {
        var list = new List<Entry>();

        _log.Debug("Start connecting to mime-db data.");
        byte[] bytes = _httpClient.GetByteArrayAsync(MIME_DB_URL).Result;
        _log.Debug("Successfully connected to mime-db data.");
        var entry = new MimeDBEntry();
        Utf8JsonReader reader = new Utf8JsonReader(bytes);

        string mimeType = "";

        while (reader.Read())
        {
            string s = Encoding.UTF8.GetString(reader.ValueSpan);

            if (reader.CurrentDepth == 1 && reader.TokenType == JsonTokenType.PropertyName)
            {
                entry.Clear();
                entry.MimeType = s;
            }
            else if(reader.CurrentDepth == 2 && reader.TokenType == JsonTokenType.PropertyName && StringComparer.Ordinal.Equals("extensions", s))
            {

            }
        }

        

        return list;
    }
}
