
using System;
using System.ComponentModel;
using System.Globalization;
using andecr.Controls;

namespace andecr.Converters;

public class TileSizeTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? ctx, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(ctx, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext? ctx,
        CultureInfo? culture, object value)
        => value is string s ? TileSize.Parse(s) : base.ConvertFrom(ctx, culture, value)!;
}