namespace FluentFiles.FixedLength.Implementation
{
    using System;
    using System.Linq.Expressions;
    using FluentFiles.Core;
    using FluentFiles.Core.Base;

    /// <summary>
    /// A mapping from a fixed-length file record to a target type.
    /// </summary>
    /// <typeparam name="TTarget">The type that a record maps to.</typeparam>
    public class FixedLayout<TTarget> :
        LayoutBase<TTarget, IFixedFieldSettingsContainer, IFixedFieldSettingsBuilder, IFixedLayout<TTarget>>,
        IFixedLayout<TTarget>
    {
        /// <summary>
        /// Initializes a new <see cref="FixedLayout{TTarget}"/>.
        /// </summary>
        public FixedLayout()
            : this(new FixedFieldSettingsBuilderFactory(),
                   new FieldCollection<IFixedFieldSettingsContainer>())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="FixedLayout{TTarget}"/>.
        /// </summary>
        /// <param name="fieldSettingsFactory">Creates fixed-length field configurations.</param>
        /// <param name="fieldCollection">Stores the field configurations in a layout.</param>
        public FixedLayout(
            IFieldSettingsBuilderFactory<IFixedFieldSettingsBuilder, IFixedFieldSettingsContainer> fieldSettingsFactory,
            IFieldCollection<IFixedFieldSettingsContainer> fieldCollection)
                : base(fieldSettingsFactory, fieldCollection)
        {
        }

        /// <summary>
        /// Configures a mapping from a record field to a member of a type.
        /// </summary>
        /// <typeparam name="TMember">The type of the member a field maps to.</typeparam>
        /// <param name="expression">An expression selecting the member to map to.</param>
        /// <param name="configure">An action that performs configuration of a field mapping.</param>
        public override IFixedLayout<TTarget> WithMember<TMember>(
            Expression<Func<TTarget, TMember>> expression,
            Action<IFixedFieldSettingsBuilder>? configure = null)
        {
            ProcessMember(expression, configure);

            return this;
        }

        /// <summary>
        /// Indicates that a record layout contains a header.
        /// </summary>
        /// <returns></returns>
        public override IFixedLayout<TTarget> WithHeader()
        {
            HasHeader = true;

            return this;
        }

        /// <summary>
        /// Ignores a fixed width section of a record. When writing, the field will be filled with spaces
        /// by default, or an alternate character can be chosen using the configuration action.
        /// </summary>
        /// <param name="length">The length of the section to ignore.</param>
        /// <param name="configure">An action that can be used to further configure the ignored field.</param>
        public IFixedLayout<TTarget> Ignore(int length, Action<IIgnoredFieldBuilder>? configure)
        {
            var builder = new IgnoredFieldBuilder();
            configure?.Invoke(builder);

            FieldCollection.AddOrUpdate(new IgnoredFixedFieldSettings(length, builder.Filler));
            return this;
        }

        private class IgnoredFieldBuilder : IIgnoredFieldBuilder
        {
            public char Filler { get; private set; } = ' ';

            public IIgnoredFieldBuilder FillWith(char filler)
            {
                Filler = filler;
                return this;
            }
        }
    }
}