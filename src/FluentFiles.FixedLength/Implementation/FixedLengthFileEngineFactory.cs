namespace FluentFiles.FixedLength.Implementation
{
    using System;
    using System.Collections.Generic;
    using FluentFiles.Core;
    using FluentFiles.Core.Base;

    /// <summary>
    /// Class FixedLengthFileEngineFactory.
    /// </summary>
    public class FixedLengthFileEngineFactory : IFlatFileEngineFactory<IFixedLengthLayoutDescriptor, IFixedFieldSettingsContainer>
    {
        readonly FixedLengthLineParserFactory lineParserFactory = new FixedLengthLineParserFactory();

        /// <summary>
        /// Registers the line parser <typeparamref name="TParser" /> for lines matching <paramref name="targetType" />.
        /// </summary>
        /// <typeparam name="TParser">The type of the t parser.</typeparam>
        /// <param name="targetType">The target record type.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RegisterLineParser<TParser>(Type targetType) where TParser : IFixedLengthLineParser
        {
            lineParserFactory.RegisterLineParser<TParser>(targetType);
        }

        /// <summary>
        /// Registers the line parser <typeparamref name="TParser" /> for lines matching <paramref name="targetLayout" />.
        /// </summary>
        /// <typeparam name="TParser">The type of the t parser.</typeparam>
        /// <param name="targetLayout">The target layout.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RegisterLineParser<TParser>(ILayoutDescriptor<IFieldSettings> targetLayout) where TParser : IFixedLengthLineParser
        {
            lineParserFactory.RegisterLineParser<TParser>(targetLayout);
        }

        /// <summary>
        /// Gets the <see cref="IFlatFileEngine" />.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="handleEntryReadError">The handle entry read error func.</param>
        /// <returns>IFlatFileEngine.</returns>
        public IFlatFileEngine GetEngine(
            IFixedLengthLayoutDescriptor descriptor,
            FileReadErrorHandler? handleEntryReadError = null)
        {
            return new FixedLengthFileEngine(
                descriptor,
                new FixedLengthLineBuilderFactory(),
                lineParserFactory,
                handleEntryReadError);
        }

        /// <summary>
        /// Gets the <see cref="IFlatFileMultiEngine"/>.
        /// </summary>
        /// <param name="layoutDescriptors">The layout descriptors.</param>
        /// <param name="typeSelectorFunc">The type selector function.</param>
        /// <param name="handleEntryReadError">The handle entry read error func.</param>
        /// <param name="masterDetailStrategy">Determines how master-detail record relationships are handled.</param>
        /// <returns>IFlatFileMultiEngine.</returns>
        public IFlatFileMultiEngine GetEngine(
            IEnumerable<IFixedLengthLayoutDescriptor> layoutDescriptors,
            Func<string, int, Type> typeSelectorFunc,
            FileReadErrorHandler? handleEntryReadError = null,
            IMasterDetailStrategy? masterDetailStrategy = null)
        {
            return new FixedLengthFileMultiEngine(
                layoutDescriptors,
                typeSelectorFunc,
                new FixedLengthLineBuilderFactory(),
                lineParserFactory,
                masterDetailStrategy ?? new DefaultFixedLengthMasterDetailStrategy(),
                handleEntryReadError);
        }
    }
}