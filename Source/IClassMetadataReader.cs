using System.Collections.Generic;

namespace DelimitedStringParser
{
    public interface IClassMetadataReader<T>
    {
        bool IsVersionedData { get; }

        string Delimiter { get; } 
        
        IEnumerable<FieldMetadata> FieldList { get; }
    }
}
