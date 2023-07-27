using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public readonly partial struct MimeTypeParameterInfo
{

    /// <inheritdoc/>
    public override string ToString()
    {
        return base.ToString() ?? nameof(MimeTypeParameterInfo);
    }


    ///// <summary>
    ///// Creates a <see cref="string"/> representation of the instance.
    ///// </summary>
    ///// <returns>A <see cref="string"/> representation of the instance.</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public override string ToString() => _parameterString.ToString();
}
