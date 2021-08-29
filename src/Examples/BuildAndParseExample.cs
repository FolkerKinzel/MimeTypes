using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;

namespace Examples
{
    public static class BuildAndParseExample
    {
        public static void Example()
        {
            var dic = new MimeTypeParameterModelDictionary()
            {
                new MimeTypeParameterModel("first-parameter",
                "This is a very long parameter, which will be wrapped according to RFC 2184." +
                Environment.NewLine +
                "It contains also a few Non-ASCII-Characters: \u00E4\u00D6\u00DF.", "en"),
                new MimeTypeParameterModel("second-parameter", "Parameter with  \\, = and \".")
            };

            var mimeType1 = new MimeType("application", "x-stuff", dic);
            string s = mimeType1.ToString(MimeTypeFormattingOptions.LineWrapping | MimeTypeFormattingOptions.Default);

            Console.WriteLine(s);

            // Parse s:
            var mimeType2 = MimeType.Parse(s);

            Console.WriteLine();
            Console.WriteLine($"Media Type: {mimeType2.MediaType.ToString()}");
            Console.WriteLine($"Sub Type:   {mimeType2.SubType.ToString()}");

            int parameterCounter = 1;
            foreach (MimeTypeParameter parameter in mimeType2.Parameters)
            {
                Console.WriteLine();
                Console.WriteLine($"Parameter {parameterCounter++}:");
                Console.WriteLine($"  Key:      {parameter.Key.ToString()}");
                Console.WriteLine($"  Value:    {parameter.Value.ToString()}");
                Console.WriteLine($"  Language: {parameter.Language.ToString()}");

            }

        }
    }
}
