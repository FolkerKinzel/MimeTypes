using FolkerKinzel.MimeTypes.Intls;
using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    /// <summary>
    /// Serializes the instance as Internet Media Type <see cref="string"/> ("MIME type") using the 
    /// <see cref="MimeFormats.Default"/> format.
    /// </summary>
    /// <returns>A <see cref="string"/> representation of the instance 
    /// according to RFC 2045 and RFC 2231.</returns>
    /// <example>
    /// <para>Formatting a <see cref="MimeType"/> instance into a standards-compliant <see cref="string"/> using several options:</para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => ToString(MimeFormats.Default);


    /// <summary>
    /// Serializes the instance as Internet Media Type <see cref="string"/> ("MIME type") with several <paramref name="options"/>
    /// </summary>
    /// <param name="options">Named constants to specify options for the serialization of the instance. The
    /// flags can be combined.</param>
    /// <param name="lineLength">The number of characters in a single line of the serialized instance
    /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeFormats.LineWrapping"/>
    /// is not set. If the value of the argument is smaller than <see cref="MimeType.MinLineLength"/>, the value of 
    /// <see cref="MimeType.MinLineLength"/> is taken instead.</param>
    /// <returns>A <see cref="string"/> representation of the instance according to RFC 2045 and RFC 2231.</returns>
    /// <example>
    /// <para>Formatting a <see cref="MimeType"/> instance into a standards-compliant <see cref="string"/> using several options:</para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
    /// </example>
    public string ToString(MimeFormats options, int lineLength = MimeType.MinLineLength)
        => !HasParameters || options.HasFlag(MimeFormats.IgnoreParameters)
            ? MediaType + '/' + SubType
            : AppendToInternal(new StringBuilder(), options, lineLength).ToString();


    /// <summary>
    /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2231 to the end of a 
    /// <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <param name="options">Named constants to specify options for the serialization of the instance. The
    /// flags can be combined.</param>
    /// <param name="maxLineLength">The number of characters in a single line of the serialized instance
    /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeFormats.LineWrapping"/>
    /// is not set. If the value of the argument is smaller than <see cref="MimeType.MinLineLength"/>, the value of 
    /// <see cref="MimeType.MinLineLength"/> is taken instead.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder,
                                  MimeFormats options = MimeFormats.Default,
                                  int maxLineLength = MimeType.MinLineLength) => 
        builder is null ? throw new ArgumentNullException(nameof(builder))
                        : AppendToInternal(builder, options, maxLineLength);


    private StringBuilder AppendToInternal(StringBuilder builder,
                                           MimeFormats options,
                                           int maxLineLength)
    {

        Debug.Assert(builder != null);
        options = options.Normalize();

        int startOfMimeType = builder.Length;

        _ = builder.EnsureCapacity(builder.Length + 
                                   MediaType.Length + 
                                   1 + // "/"
                                   SubType.Length + 
                                   (HasParameters ? _dic.Count : 0) * MimeTypeParameter.STRING_LENGTH);
        _ = builder.Append(MediaType).Append('/').Append(SubType);
        

        if (options != MimeFormats.IgnoreParameters)
        {
            if (options.HasFlag(MimeFormats.LineWrapping))
            {
                AppendWrappedParameters(builder, options, maxLineLength, startOfMimeType);
            }
            else
            {
                AppendUnWrappedParameters(builder, options);
            }
        }

        return builder;
    }


    private void AppendUnWrappedParameters(StringBuilder builder, MimeFormats options)
    {
        Debug.Assert(options == options.Normalize());
        bool appendSpace = !options.HasFlag(MimeFormats.AvoidSpace);

        foreach (MimeTypeParameter parameter in Parameters)
        {
            _ = builder.Append(';');

            if (appendSpace)
            {
                _ = builder.Append(' ');
            }

            ParameterSerializer.AppendTo(builder, parameter, options == MimeFormats.Url);
        }
    }


    private void AppendWrappedParameters(StringBuilder builder,
                                         MimeFormats options,
                                         int maxLineLength,
                                         int startOfMimeType)
    {
        Debug.Assert(options == options.Normalize());

        MimeTypeSerializer.NormalizeIndexes(builder,
                                            startOfMimeType,
                                            ref maxLineLength,
                                            out int startOfCurrentLine);

        var worker = new StringBuilder(maxLineLength * 3);
        int currentLineLength = builder.Length - startOfCurrentLine;


        foreach (MimeTypeParameter parameter in Parameters)
        {
            EncodingAction action = ParameterSerializer.AppendTo(worker.Clear(), parameter, false);

            int keyLength = parameter.Key.Length;
            int languageLength = parameter.Language?.Length ?? 0;

            currentLineLength = ParameterSerializer.SplitParameter(builder,
                                                                   worker,
                                                                   maxLineLength,
                                                                   keyLength,
                                                                   languageLength,
                                                                   !options.HasFlag(MimeFormats.AvoidSpace),
                                                                   action,
                                                                   currentLineLength);
        }
    }

}
