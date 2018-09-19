namespace FluentFiles.Core.Extensions
{
    using FluentFiles.Core.Conversion;
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    internal static class TypeExtensions
    {
        public static IValueConverter GetConverter(this Type type)
        {
            if (Registry.Converters.TryGetValue(type, out var registered))
                return registered;

            var converter = TypeDescriptor.GetConverter(type.Unwrap());
            return converter != null 
                ? new TypeConverterAdapter(converter) { OverrideCanConvertTo = true }
                : DefaultValueConverter.Instance;
        }

        public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        public static Type Unwrap(this Type type) => type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;

        public static object GetDefaultValue(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // We want an Func<object> which returns the default.
            // Create that expression here.
            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    type.GetDefaultExpression(), typeof(object)
                    )
                );

            // Compile and return the value.
            return e.Compile()();
        }

        public static Expression GetDefaultExpression(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsValueType)
            {
                return Expression.Constant(Activator.CreateInstance(type), type);
            }

            return Expression.Constant(null, type);
        }
    }
}