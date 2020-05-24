namespace FluentFiles.FixedLength
{
    using System;
    using System.Reflection;
    using FluentFiles.Core.Conversion;

    internal class IgnoredFixedFieldSettings : IFixedFieldSettingsContainer
    {
        private readonly char _filler;

        public IgnoredFixedFieldSettings(int length, char filler)
        {
            Length = length;
            _filler = filler;
            UniqueKey = Guid.NewGuid().ToString();
        }
        
        public int? Index { get; set; }

        public int Length { get; }
        public string UniqueKey { get; }
        public bool IsNullable { get; } = false;
        public string? NullValue { get; } = null;
        public IFieldValueConverter? Converter { get; } = null;
        public bool PadLeft { get; } = false;
        public char PaddingChar { get; } = default;

        public Func<char, int, bool>? SkipWhile { get; } = null;
        public Func<char, int, bool>? TakeUntil { get; } = null;

        public bool TruncateIfExceedFieldLength { get; } = false;
        public Func<string, string>? StringNormalizer { get; } = null;
        public Type Type { get; } = typeof(string);
        public MemberInfo Member => throw new NotSupportedException("Ignored fields have no member mapped.");

        public object? GetValueOf(object instance) => new string(_filler, Length);
        public void SetValueOf(object instance, object? value) { /* no-op */ }
    }
}