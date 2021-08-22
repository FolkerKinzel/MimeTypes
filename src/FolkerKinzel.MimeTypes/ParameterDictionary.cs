using System;
using System.Collections.ObjectModel;

namespace FolkerKinzel.MimeTypes
{
    /// <summary>
    /// A Dictionary for the <see cref="ParameterModel"/> structure. <see cref="ParameterModel.Key"/> 
    /// is used as the Dictionary-Key. The key comparison is not case sensitive. The elements in the
    /// Dictionary keep the order in which they are inserted.
    /// </summary>
    public class ParameterDictionary : KeyedCollection<string, ParameterModel>
    {
        /// <summary>
        /// Initializes a new <see cref="ParameterDictionary"/> object.
        /// </summary>
        public ParameterDictionary() : base(StringComparer.OrdinalIgnoreCase, -1)
        {

        }

        
        /// <inheritdoc/>
        protected override string GetKeyForItem(ParameterModel item) => item.Key;

    }
}
