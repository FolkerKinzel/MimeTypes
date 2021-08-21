using System;
using System.Collections.ObjectModel;

namespace FolkerKinzel.MimeTypes
{
    /// <summary>
    /// A Dictionary for the <see cref="MimeTypeParameterData"/> structure. <see cref="MimeTypeParameterData.Key"/> 
    /// is used as the Dictionary-Key. The key comparison is not case sensitive. The elements in the
    /// Dictionary keep the order in which they are inserted.
    /// </summary>
    public class MimeTypeParameterDataDictionary : KeyedCollection<string, MimeTypeParameterData>
    {
        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameterDataDictionary"/> object.
        /// </summary>
        public MimeTypeParameterDataDictionary() : base(StringComparer.OrdinalIgnoreCase, -1)
        {

        }

        
        /// <inheritdoc/>
        protected override string GetKeyForItem(MimeTypeParameterData item) => item.Key;

    }
}
