# Changelog

## Version 1.0.0

### New Features
- Support .NET Core and target .NET Standard.

- Expose an extensibility point for customizing how master/detail records are handled through the interface 
  `IMasterDetailStrategy`.

- Provide a new value converter mechanism, `IFieldValueConverter`, to replace `ITypeConverter`
  and expose more information such as the `PropertyInfo` of the property being used.

- Use field value converters when building lines.

- Provide field builder methods (`WithConversionFromString`, `WithConversionToString`) that accept 
  delegates for performing field value conversion.

- Provide fixed length field builder methods (`SkipWhile`, `TakeUntil`) for advanced field value processing.

- [#41](https://github.com/forcewake/FlatFile/issues/41), [#67](https://github.com/forcewake/FlatFile/issues/67) Enable capability to designate that certain sections of fixed length files should be ignored.

- [#63](https://github.com/forcewake/FlatFile/issues/62) Expose `Read` and `Write` file engine methods that 
  accept `TextReader` and `TextWriter`, respectively.

- Support public class fields in addition to properties.

- Changed file engine write and multi-record read to be asynchronous. Single-record-type read is still synchronous for now.


### Bug Fixes
- [#80](https://github.com/forcewake/FlatFile/issues/80) Problem parsing consecutive empty fields in delimited files.
