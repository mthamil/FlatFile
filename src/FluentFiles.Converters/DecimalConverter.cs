﻿using FluentFiles.Core.Conversion;
using System;
using System.Globalization;

namespace FluentFiles.Converters
{
    public class DecimalConverter : NumberConverterBase<decimal>
    {
        protected override decimal ConvertFromString(ReadOnlySpan<char> source, NumberFormatInfo format) =>
            decimal.Parse(source, NumberStyles.Float, provider: format);

        protected override ReadOnlySpan<char> ConvertToString(decimal value, NumberFormatInfo format) =>
            value.ToString("G", format);
    }
}