using FolkerKinzel.MimeTypes;

namespace Examples;

public static class EqualityExample2
{
    public static void Example()
    {
        const string media1 = "text/plain; charset=us-ascii";
        const string media2 = "TEXT/PLAIN";
        const string media3 = "TEXT/HTML";
        const string media4 = "text/plain; charset=iso-8859-1";
        const string media5 = "TEXT/PLAIN; CHARSET=ISO-8859-1";
        const string media6 = "text/plain; charset=iso-8859-1; other-parameter=other_value";
        const string media7 = "text/plain; OTHER-PARAMETER=other_value; charset=ISO-8859-1";
        const string media8 = "text/plain; charset=iso-8859-1; other-parameter=OTHER_VALUE";

        if (MimeType.Parse(media1) == MimeType.Parse(media2) &&
            MimeType.Parse(media2) != MimeType.Parse(media3) &&
            MimeType.Parse(media2) != MimeType.Parse(media4) &&
            MimeType.Parse(media2).Equals(MimeType.Parse(media4), ignoreParameters: true) &&
            MimeType.Parse(media4) == MimeType.Parse(media5) &&
            MimeType.Parse(media4) != MimeType.Parse(media6) &&
            MimeType.Parse(media6) == MimeType.Parse(media7) &&
            MimeType.Parse(media6) != MimeType.Parse(media8))
        {
            Console.WriteLine("Success");
        }
        else
        {
            Console.WriteLine("Error");
        }
    }
}
// Console Output: Success
