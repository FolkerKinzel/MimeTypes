using System.Collections.ObjectModel;

namespace FolkerKinzel.MimeTypes;

/// <summary>
/// A Dictionary for the <see cref="MimeTypeParameterModel"/> structure. <see cref="MimeTypeParameterModel.Key"/> 
/// is used as the Dictionary-Key. The key comparison is not case sensitive. The elements in the
/// Dictionary keep the order in which they are inserted.
/// </summary>
public class MimeTypeParameterModelDictionary : KeyedCollection<string, MimeTypeParameterModel>
{
    /// <summary>
    /// Initializes a new <see cref="MimeTypeParameterModelDictionary"/> object.
    /// </summary>
    public MimeTypeParameterModelDictionary() : base(StringComparer.OrdinalIgnoreCase, -1) { }
    

    /// <inheritdoc/>
    protected override string GetKeyForItem(MimeTypeParameterModel item) => item.Key;

}
