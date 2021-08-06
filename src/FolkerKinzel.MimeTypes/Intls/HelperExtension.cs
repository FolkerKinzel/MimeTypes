using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Strings;

namespace FolkerKinzel.MimeTypes.Intls
{
    internal static class HelperExtension
    {
        internal static bool ContainsTSpecials(this ReadOnlySpan<char> span)
            // RFC 2045 Section 5.1 "tspecials"
            => span.ContainsAny(stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' });
    }
}
