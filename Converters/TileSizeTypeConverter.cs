using System;
using System.ComponentModel;
using System.Globalization;
using andecr.Controls;

namespace andecr.Converters;

/// <summary>
/// A <see cref="TypeConverter"/> that converts string representations to <see cref="TileSize"/> values.
/// </summary>
/// <remarks>
/// <para>
/// Avalonia's AXAML parser invokes this converter automatically when it encounters a
/// <see cref="TileSize"/>-typed property written as a string attribute in markup, for example:
/// <code>
/// &lt;controls:TileControl TileWidth="120px" TileHeight="50%" /&gt;
/// &lt;controls:TileControl TileWidth="Auto" /&gt;
/// </code>
/// </para>
/// <para>
/// The converter is registered on <see cref="TileSize"/> via the
/// <see cref="System.ComponentModel.TypeConverterAttribute"/> and should not normally need to be
/// referenced directly in application code.
/// </para>
/// <para>
/// Accepted string formats are delegated to <see cref="TileSize.Parse"/>:
/// <list type="bullet">
///   <item><description><c>"Auto"</c> or empty/whitespace → <see cref="TileSize.Auto"/></description></item>
///   <item><description><c>"100px"</c> or <c>"100"</c> → <c>TileSize.FromPixels(100)</c></description></item>
///   <item><description><c>"50%"</c> → <c>TileSize.FromPercent(50)</c></description></item>
/// </list>
/// </para>
/// </remarks>
public class TileSizeTypeConverter : TypeConverter
{
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to a <see cref="TileSize"/>.
    /// </summary>
    /// <param name="ctx">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context, or <c>null</c>.
    /// </param>
    /// <param name="sourceType">The type to convert from.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="sourceType"/> is <see cref="string"/>;
    /// otherwise the result of the base implementation.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? ctx, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(ctx, sourceType);

    /// <summary>
    /// Converts the given value to a <see cref="TileSize"/>.
    /// </summary>
    /// <param name="ctx">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context, or <c>null</c>.
    /// </param>
    /// <param name="culture">
    /// The <see cref="CultureInfo"/> to use for culture-sensitive operations.
    /// Note that <see cref="TileSize.Parse"/> always uses the invariant culture for number parsing,
    /// so this parameter does not affect the numeric conversion.
    /// </param>
    /// <param name="value">The object to convert. Must be a <see cref="string"/> for this converter to handle it.</param>
    /// <returns>
    /// A <see cref="TileSize"/> parsed from <paramref name="value"/> when it is a <see cref="string"/>;
    /// otherwise the result of the base implementation.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown by <see cref="TileSize.Parse"/> when <paramref name="value"/> is a string that does not
    /// match any accepted format (<c>"Auto"</c>, <c>"100px"</c>, <c>"50%"</c>, etc.).
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext? ctx,
        CultureInfo? culture, object value)
        => value is string s ? TileSize.Parse(s) : base.ConvertFrom(ctx, culture, value)!;
}