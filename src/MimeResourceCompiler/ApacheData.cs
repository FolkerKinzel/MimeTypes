﻿using MimeResourceCompiler.Classes;

namespace MimeResourceCompiler;

/// <summary>
/// Represents the Apache file http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types.
/// </summary>
public interface IApacheData : IDisposable
{
    /// <summary>
    /// Gets the next line with data from the apache file, or null if the file is completely read.
    /// </summary>
    /// <returns>The next line with data from the apache file as a collection of <see cref="Entry"/> objects
    /// or null if the file is completely read.</returns>
    IEnumerable<Entry>? GetNextLine();
}

/// <summary>
/// Represents the Apache file http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types.
/// </summary>
public sealed class ApacheData : IApacheData, IDisposable
{
    private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";

    private readonly HttpClient _httpClient;
    private StreamReader? _reader;
    private readonly ILogger _log;
    private readonly List<Entry> _list = new(8);
    private bool _disposedValue;

    /// <summary>
    /// ctor
    /// </summary>
    public ApacheData(HttpClient client, ILogger log)
    {
        _httpClient = client;
        _log = log;
    }

    /// <summary>
    /// Gets the next line with data from the apache file, or null if the file is completely read.
    /// </summary>
    /// <returns>The next line with data from the apache file or null if the file is completely read.</returns>
    public IEnumerable<Entry>? GetNextLine()
    {
        _reader ??= InitReader();
        string? line;

        while ((line = _reader.ReadLine()) is not null)
        {
            line = line.Trim();
            if (line.StartsWith('#') || line.Length == 0)
            {
                continue;
            }

            if (AddApacheLine(line))
            {
                if (_list.Count != 0)
                {
                    return _list;
                }
            }
        }

        return null;
    }

    private StreamReader InitReader()
    {
        _log.Debug("Start connecting to Apache data.");
        Stream data = _httpClient.GetStreamAsync(APACHE_URL).Result;
        _log.Debug("Apache data successfully connected.");
        return new StreamReader(data);
    }

    private bool AddApacheLine(string line)
    {
        string[] parts = Regexes.WhiteSpace().Split(line);

        if (parts.Length < 2)
        {
            return false;
        }

        _list.Clear();

        for (int i = 1; i < parts.Length; i++)
        {
            _list.Add(new Entry(parts[0], parts[i]));
        }

        return true;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                _reader?.Close();
                _log.Debug("Apache file closed.");
            }

            // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            // TODO: Große Felder auf NULL setzen
            _disposedValue = true;
        }
    }

    // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    // ~ApacheData()
    // {
    //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ///// <summary>
    ///// Releases the resources.
    ///// </summary>
    //public void Dispose()
    //{
    //    _reader?.Close();
    //    GC.SuppressFinalize(this);
    //    _log.Debug("Apache file closed.");
    //}
}

