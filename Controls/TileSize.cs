using System;
using System.Globalization;
using andecr.Converters;

namespace andecr.Controls;

/// <summary>
/// Represents the size of a tile, which can be specified in pixels, as a percentage,
/// or as automatic (matching the original image size along the given axis).
/// </summary>
/// <remarks>
/// <para>
/// This is an immutable <c>readonly struct</c> that supports three sizing modes:
/// <list type="bullet">
///   <item><description><b>Auto</b> — the size equals the original image size along the given axis (default value).</description></item>
///   <item><description><b>Pixels</b> — a fixed size in logical pixels.</description></item>
///   <item><description><b>Percent</b> — a size proportional to the container control's size along the given axis.</description></item>
/// </list>
/// </para>
/// <para>
/// In AXAML markup the value is supplied as a string attribute and is automatically
/// converted via <see cref="TileSizeTypeConverter"/>:
/// <code>
/// &lt;controls:TileControl TileWidth="120px" TileHeight="50%" /&gt;
/// &lt;controls:TileControl TileWidth="Auto" /&gt;
/// </code>
/// </para>
/// <para>
/// In C# code an implicit conversion from <see cref="double"/> is available (produces a pixel value),
/// as well as the factory methods <see cref="FromPixels"/> and <see cref="FromPercent"/>:
/// <code>
/// TileSize a = 100.0;                    // 100px
/// TileSize b = TileSize.FromPercent(50); // 50%
/// TileSize c = TileSize.Auto;            // Auto
/// </code>
/// </para>
/// </remarks>
[System.ComponentModel.TypeConverter(typeof(TileSizeTypeConverter))]
public readonly struct TileSize : IEquatable<TileSize>
{
    private enum Kind { Auto, Pixels, Percent }
    private readonly Kind   _mode;
    private readonly double _value;

    /// <summary>
    /// The <see cref="TileSize"/> value representing the <c>Auto</c> mode.
    /// When the size is resolved via <see cref="Resolve"/>, it returns the original image size along the given axis.
    /// </summary>
    public static readonly TileSize Auto = default;

    /// <summary>Returns <c>true</c> if the size is in <c>Auto</c> mode.</summary>
    public bool IsAuto    => _mode == Kind.Auto;

    /// <summary>Returns <c>true</c> if the size is specified in logical pixels.</summary>
    public bool IsPixels  => _mode == Kind.Pixels;

    /// <summary>Returns <c>true</c> if the size is specified as a percentage of the container's size.</summary>
    public bool IsPercent => _mode == Kind.Percent;

    /// <inheritdoc/>
    public bool Equals(TileSize other) =>
        _mode == other._mode &&
        BitConverter.DoubleToInt64Bits(_value) == BitConverter.DoubleToInt64Bits(other._value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TileSize other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(_mode, BitConverter.DoubleToInt64Bits(_value));

    /// <summary>Determines whether two <see cref="TileSize"/> values are equal.</summary>
    public static bool operator ==(TileSize left, TileSize right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="TileSize"/> values are not equal.</summary>
    public static bool operator !=(TileSize left, TileSize right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a <see cref="double"/> to a <see cref="TileSize"/> in pixel mode.
    /// </summary>
    /// <param name="px">The size in logical pixels.</param>
    /// <example>
    /// <code>TileSize size = 100.0; // equivalent to TileSize.FromPixels(100)</code>
    /// </example>
    public static implicit operator TileSize(double px) => FromPixels(px);

    /// <summary>
    /// Creates a <see cref="TileSize"/> with a fixed size in logical pixels.
    /// </summary>
    /// <param name="px">The size in logical pixels. Should be non-negative.</param>
    /// <returns>A <see cref="TileSize"/> instance in <c>Pixels</c> mode.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static TileSize FromPixels(double px) => new(Kind.Pixels,  px);

    /// <summary>
    /// Creates a <see cref="TileSize"/> as a percentage of the container's size.
    /// </summary>
    /// <param name="pct">The percentage of the container's size (e.g. <c>50</c> means 50%).</param>
    /// <returns>A <see cref="TileSize"/> instance in <c>Percent</c> mode.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static TileSize FromPercent(double pct) => new(Kind.Percent, pct);

    private TileSize(Kind mode, double value)
    {
        _mode = mode; 
        _value = value;
    }

    /// <summary>
    /// Resolves the final size in logical pixels according to the current mode.
    /// </summary>
    /// <param name="originalSize">
    /// The original image size along the given axis (width or height in pixels).
    /// Used in <c>Auto</c> mode.
    /// </param>
    /// <param name="containerSize">
    /// The size of the container control along the given axis in logical pixels.
    /// Used only in <c>Percent</c> mode.
    /// </param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description><c>Auto</c> → <paramref name="originalSize"/></description></item>
    ///   <item><description><c>Pixels</c> → the fixed pixel value</description></item>
    ///   <item><description><c>Percent</c> → <c>containerSize * value / 100</c></description></item>
    /// </list>
    /// </returns>
    public double Resolve(double originalSize, double containerSize) => _mode switch
    {
        Kind.Pixels  => _value,
        Kind.Percent => containerSize * _value / 100.0,
        _            => originalSize
    };

    /// <summary>
    /// Attempts to parse a string representation into a <see cref="TileSize"/> value.
    /// </summary>
    /// <param name="s">
    /// The string to parse. Accepted formats:
    /// <list type="bullet">
    ///   <item><description><c>null</c>, empty string, or whitespace → <see cref="Auto"/></description></item>
    ///   <item><description><c>"Auto"</c> → <see cref="Auto"/></description></item>
    ///   <item><description><c>"100px"</c> or <c>"100"</c> → <c>FromPixels(100)</c></description></item>
    ///   <item><description><c>"50%"</c> → <c>FromPercent(50)</c></description></item>
    /// </list>
    /// Numbers are parsed using the invariant culture (decimal separator is a period).
    /// </param>
    /// <param name="result">
    /// On success — the parsed <see cref="TileSize"/> value;
    /// on failure — <see cref="Auto"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the string was parsed successfully; otherwise <c>false</c>.
    /// </returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryParse(string? s, out TileSize result)
    {
        result = Auto;
        if (string.IsNullOrWhiteSpace(s)) return true;

        s = s.Trim();

        // ← добавить эту строку
        if (s.Equals("Auto", StringComparison.OrdinalIgnoreCase)) return true;

        if (s.EndsWith('%'))
            return double.TryParse(s.TrimEnd('%'), NumberStyles.Float,
                       CultureInfo.InvariantCulture, out var pct)
                   && (result = FromPercent(pct)) is var _;

        var numStr = s.EndsWith("px", StringComparison.OrdinalIgnoreCase) ? s[..^2] : s;
        if (!double.TryParse(numStr, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var px)) return false;
        result = FromPixels(px);
        return true;
    }

    /// <summary>
    /// Parses a string representation into a <see cref="TileSize"/> value.
    /// Called by Avalonia when parsing AXAML attributes via <see cref="TileSizeTypeConverter"/>.
    /// </summary>
    /// <param name="s">
    /// The string to parse. Accepted formats: <c>"100px"</c>, <c>"50%"</c>, <c>"Auto"</c>
    /// (or an empty string, which is equivalent to <c>Auto</c>).
    /// </param>
    /// <returns>The parsed <see cref="TileSize"/> value.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the string does not match any of the accepted formats.
    /// </exception>
    public static TileSize Parse(string s) =>
        TryParse(s, out var result)
            ? result
            : throw new FormatException($"Invalid TileSize: '{s}'. Expected '100px', '50%', or 'Auto'.");

    /// <summary>
    /// Returns the string representation of the value in the native AXAML format.
    /// </summary>
    /// <returns>
    /// <c>"Auto"</c>, <c>"100px"</c>, or <c>"50%"</c> depending on the mode.
    /// The result is a valid input for <see cref="Parse"/> (round-trip safe).
    /// </returns>
    public override string ToString() => _mode switch
    {
        Kind.Pixels  => $"{_value}px",
        Kind.Percent => $"{_value}%",
        _            => "Auto"
    };
}