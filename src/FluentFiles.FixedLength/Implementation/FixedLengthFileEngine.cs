namespace FluentFiles.FixedLength.Implementation
{
    using System;
    using FluentFiles.Core;
    using FluentFiles.Core.Base;

    /// <summary>
    /// Class FixedLengthFileEngine.
    /// </summary>
    public class FixedLengthFileEngine : FlatFileEngine<IFixedFieldSettingsContainer, IFixedLengthLayoutDescriptor>
    {
        /// <summary>
        /// The line builder factory
        /// </summary>
        private readonly IFixedLengthLineBuilderFactory lineBuilderFactory;
        /// <summary>
        /// The line parser factory
        /// </summary>
        private readonly IFixedLengthLineParserFactory lineParserFactory;
        /// <summary>
        /// The layout descriptor
        /// </summary>
        private readonly IFixedLengthLayoutDescriptor layoutDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedLengthFileEngine"/> class.
        /// </summary>
        /// <param name="layoutDescriptor">The layout descriptor.</param>
        /// <param name="lineBuilderFactory">The line builder factory.</param>
        /// <param name="lineParserFactory">The line parser factory.</param>
        /// <param name="handleEntryReadError">The handle entry read error.</param>
        internal FixedLengthFileEngine(
            IFixedLengthLayoutDescriptor layoutDescriptor,
            IFixedLengthLineBuilderFactory lineBuilderFactory,
            IFixedLengthLineParserFactory lineParserFactory,
            FileReadErrorHandler handleEntryReadError = null) : base(handleEntryReadError)
        {
            this.lineBuilderFactory = lineBuilderFactory;
            this.lineParserFactory = lineParserFactory;
            this.layoutDescriptor = new FixedLengthImmutableLayoutDescriptor(layoutDescriptor);
        }

        /// <summary>
        /// Gets the line builder.
        /// </summary>
        /// <value>The line builder.</value>
        protected override ILineBuilder LineBuilder
        {
            get { return lineBuilderFactory.GetBuilder(LayoutDescriptor); }
        }

        /// <summary>
        /// Gets the line parser.
        /// </summary>
        /// <value>The line parser.</value>
        protected override ILineParser LineParser
        {
            get { return lineParserFactory.GetParser(LayoutDescriptor); }
        }

        /// <summary>
        /// Gets the layout descriptor.
        /// </summary>
        /// <value>The layout descriptor.</value>
        protected override IFixedLengthLayoutDescriptor LayoutDescriptor
        {
            get { return layoutDescriptor; }
        }
    }
}