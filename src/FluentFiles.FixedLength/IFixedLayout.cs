namespace FluentFiles.FixedLength
{
    using System;
    using FluentFiles.Core;

    /// <summary>
    /// Describes the mapping from a fixed-length file record to a target type.
    /// </summary>
    /// <typeparam name="TTarget">The type that a record maps to.</typeparam>
    public interface IFixedLayout<TTarget> :
        IFixedLengthLayoutDescriptor,
        ILayout<TTarget, IFixedFieldSettingsContainer, IFixedFieldSettingsBuilder, IFixedLayout<TTarget>>
    {
        /// <summary>
        /// Ignores a fixed width section of a record. When writing, the field will be filled with spaces
        /// by default, or an alternate character can be chosen using the configuration action.
        /// </summary>
        /// <param name="length">The length of the section to ignore.</param>
        /// <param name="configure">An action that can be used to further configure the ignored field.</param>
        IFixedLayout<TTarget> Ignore(int length, Action<IIgnoredFieldBuilder>? configure = null);
    }

    /// <summary>
    /// Provides configuration for an ignored field.
    /// </summary>
    public interface IIgnoredFieldBuilder
    {
        /// <summary>
        /// Specifies that on write, an ignored field will have the given character repeated for the length of the field.
        /// </summary>
        /// <param name="filler">The character to fill an ignored field with when writing.</param>
        public IIgnoredFieldBuilder FillWith(char filler);
    }
}