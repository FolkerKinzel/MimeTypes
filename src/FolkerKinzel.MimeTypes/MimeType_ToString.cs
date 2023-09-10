using FolkerKinzel.MimeTypes.Intls.Parameters.Serializers;

namespace FolkerKinzel.MimeTypes;

public sealed partial class MimeType
{
    /// <summary>
    /// Serializes the instance into an Internet Media Type <see cref="string"/> ("MIME type") using the 
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
    /// Serializes the instance into an Internet Media Type <see cref="string"/> ("MIME type") with several <paramref name="options"/>
    /// </summary>
    /// <param name="options">Named constants to specify options for the serialization of the instance. The
    /// flags can be combined.</param>
    /// <param name="lineLength">The number of characters in a single line of the serialized instance
    /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeFormats.LineWrapping"/>
    /// is not set. If the value of the argument is smaller than <see cref="MimeType.MinimumLineLength"/>, the value of 
    /// <see cref="MimeType.MinimumLineLength"/> is taken instead.</param>
    /// <returns>A <see cref="string"/> representation of the instance according to RFC 2045 and RFC 2231.</returns>
    /// <example>
    /// <para>Formatting a <see cref="MimeType"/> instance into a standards-compliant <see cref="string"/> using several options:</para>
    /// <code language="c#" source="./../../../FolkerKinzel.MimeTypes/src/Examples/FormattingOptionsExample.cs"/>
    /// </example>
    public string ToString(MimeFormats options, int lineLength = MimeType.MinimumLineLength)
    {
        if (!HasParameters)
        {
            return MediaType + '/' + SubType;
        }
        else
        {
            var sb = new StringBuilder(STRING_LENGTH);
            AppendToInternal(sb, options, lineLength);
            return sb.ToString();
        }
    }


    /// <summary>
    /// Appends a <see cref="string"/> representation of this instance according to RFC 2045 and RFC 2231 to the end of a 
    /// <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/>.</param>
    /// <param name="options">Named constants to specify options for the serialization of the instance. The
    /// flags can be combined.</param>
    /// <param name="lineLength">The number of characters in a single line of the serialized instance
    /// before a line-wrapping occurs. The parameter is ignored, if the flag <see cref="MimeFormats.LineWrapping"/>
    /// is not set. If the value of the argument is smaller than <see cref="MimeType.MinimumLineLength"/>, the value of 
    /// <see cref="MimeType.MinimumLineLength"/> is taken instead.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public StringBuilder AppendTo(StringBuilder builder,
                                  MimeFormats options = MimeFormats.Default,
                                  int lineLength = MimeType.MinimumLineLength) => 
        builder is null ? throw new ArgumentNullException(nameof(builder))
                        : AppendToInternal(builder, options, lineLength);


    private StringBuilder AppendToInternal(StringBuilder builder,
                              MimeFormats options,
                              int lineLength)
    {

        Debug.Assert(builder != null);
        options = options.Normalize();

        if (--lineLength < MimeType.MinimumLineLength)
        {
            lineLength = MimeType.MinimumLineLength - 1;
        }

        _ = builder.EnsureCapacity(builder.Length + STRING_LENGTH);
        _ = builder.Append(MediaType).Append('/').Append(SubType);

        Debug.Assert(options == options.Normalize());

        if (options != MimeFormats.IgnoreParameters)
        {
            if (options.HasFlag(MimeFormats.LineWrapping))
            {
                AppendWrappedParameters(builder, options, lineLength);
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

            builder.Append(parameter, options == MimeFormats.Url);
        }
    }


    private void AppendWrappedParameters(StringBuilder builder, MimeFormats options, int lineLength)
    {
        Debug.Assert(options == options.Normalize());
        Debug.Assert(lineLength >= MimeType.MinimumLineLength - 1);

        var worker = new StringBuilder(lineLength);
        bool appendSpace = !options.HasFlag(MimeFormats.AvoidSpace);

        foreach (MimeTypeParameter parameter in Parameters)
        {
            EncodingAction action = worker.Clear().Append(parameter, false);

            lineLength = ComputeMinimumLineLength(
                            parameter.Key.Length + (parameter.Language?.Length ?? 0),
                            lineLength,
                            action);

            if (worker.Length > lineLength)
            {
                foreach (StringBuilder tmp in ParameterSplitter.SplitParameter(parameter, worker, lineLength, action))
                {
                    _ = builder.Append(';').Append(NEW_LINE).Append(tmp);
                }
            }
            else
            {
                _ = builder.Append(';');

                int neededLength = worker.Length + builder.Length - (builder.LastIndexOf('\n') + 1);

                if (appendSpace)
                {
                    neededLength++;
                }

                if (neededLength > lineLength)
                {
                    _ = builder.Append(NEW_LINE);
                }
                else if (appendSpace)
                {
                    _ = builder.Append(' ');
                }

                _ = builder.Append(worker);
            }

        }
    }


    /// <summary>
    /// Computes the minimum length that is needed for a line. (Depends on key, charset,
    /// language and the <see cref="EncodingAction"/>.)
    /// </summary>
    /// <param name="givenLength">The length that can't be wrapped (key, language, charset).</param>
    /// <param name="desiredLineLength"></param>
    /// <param name="enc"></param>
    /// <returns>The minimum length that is needed for a line.</returns>
    private static int ComputeMinimumLineLength(int givenLength, int desiredLineLength, EncodingAction enc)
    {
        int minimumLength = givenLength + ParameterSplitter.MINIMUM_VARIABLE_LINE_LENGTH;

        if (enc == EncodingAction.UrlEncode)
        {
            minimumLength += ParameterSerializer.UTF_8.Length + 3; // *''
        }
        else if (enc.HasFlag(EncodingAction.Quote))
        {
            minimumLength += 2; // ""
        }

        if (desiredLineLength < minimumLength)
        {
            desiredLineLength = minimumLength;
        }

        return desiredLineLength;
    }

}
