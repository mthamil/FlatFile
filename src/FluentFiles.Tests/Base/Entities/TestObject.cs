namespace FluentFiles.Tests.Base.Entities
{
    using System;
    using FluentFiles.Delimited.Attributes;
    using FluentFiles.FixedLength;
    using FluentFiles.FixedLength.Attributes;

    [FixedLengthFile]
    [DelimitedFile(Delimiter = ";", Quotes = "\"")]
    public class TestObject : IEquatable<TestObject>
    {
        [FixedLengthField(1, 5, PaddingChar = '0')]
        [DelimitedField(1)]
        public int Id { get; set; }

        [FixedLengthField(2, 25, PaddingChar = ' ', Padding = Padding.Right)]
        [DelimitedField(2)]
        public string Description;

        [FixedLengthField(3, 5, PaddingChar = '0', NullValue = "=Null")]
        [DelimitedField(3, NullValue = "=Null")]
        public int? NullableInt { get; set; }

        public override int GetHashCode() => HashCode.Combine(Id, Description, NullableInt);

        public bool Equals(TestObject other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return Equals(Id, other.Id) && Equals(Description, other.Description) &&
                   Equals(NullableInt, other.NullableInt);
        }
    }
}