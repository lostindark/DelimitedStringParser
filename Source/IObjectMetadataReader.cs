using System.Collections.Generic;

namespace DelimitedStringParser
{
    public interface IObjectMetadataReader<T>
    {
        bool IsVersionedData { get; }

        /// <summary>
        /// The delimiter character separating each field.
        /// </summary>
        char Delimiter { get; }

        /// <summary>
        /// The quote character wrapping every field.
        /// If this is null, the field is not wrapped.
        /// </summary>
        char? Quote { get; }

        /// <summary>
        /// The escape character letting insert quotation characters inside a quoted field.
        /// If this is null, the quote character will be used.
        /// If the quote character is also null, the fields are not escaped (field value can not contains delimiter).
        /// </summary>
        char? Escape { get; }

        IEnumerable<FieldMetadata> FieldList { get; }
    }
}
