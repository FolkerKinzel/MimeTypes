using FolkerKinzel.MimeTypes;

namespace Examples;

public static class EqualityExample
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

        if (MimeTypeInfo.Parse(media1) == MimeTypeInfo.Parse(media2) &&
           MimeTypeInfo.Parse(media2) != MimeTypeInfo.Parse(media3) &&
           MimeTypeInfo.Parse(media2) != MimeTypeInfo.Parse(media4) &&
           MimeTypeInfo.Parse(media2).Equals(MimeTypeInfo.Parse(media4), ignoreParameters: true) &&
           MimeTypeInfo.Parse(media4) == MimeTypeInfo.Parse(media5) &&
           MimeTypeInfo.Parse(media4) != MimeTypeInfo.Parse(media6) &&
           MimeTypeInfo.Parse(media6) == MimeTypeInfo.Parse(media7) &&
           MimeTypeInfo.Parse(media6) != MimeTypeInfo.Parse(media8))
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
