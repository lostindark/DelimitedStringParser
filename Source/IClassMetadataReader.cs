using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelimitedStringParser
{
    public interface IClassMetadataReader<T>
    {
        bool IsVersionedData { get; }

        string Delimiter { get; }      
    }
}
