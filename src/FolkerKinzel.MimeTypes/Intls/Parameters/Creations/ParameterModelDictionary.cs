using System.Collections.ObjectModel;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations;

/// <summary>
/// A Dictionary for the <see cref="MimeTypeParameter"/> structure. <see cref="MimeTypeParameter.Key"/> 
/// is used as the Dictionary-Key. The key comparison is not case sensitive. The elements in the
/// Dictionary keep the order in which they are inserted.
/// </summary>
internal class ParameterModelDictionary : KeyedCollection<string, MimeTypeParameter>
{
    /// <summary>
    /// Initializes a new <see cref="ParameterModelDictionary"/> object.
    /// </summary>
    public ParameterModelDictionary() : base(StringComparer.OrdinalIgnoreCase, -1) { }


    /// <inheritdoc/>
    protected override string GetKeyForItem(MimeTypeParameter item) => item.Key;

}
