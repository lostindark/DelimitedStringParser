using System;
using System.Collections.Generic;
using System.Reflection;

namespace DelimitedStringParser
{
    /// <summary>
    /// Represent the field metadata of the target object.
    /// </summary>
    public struct FieldMetadata
    {
        public string Info;

        public int LookupIndex;

        public Dictionary<int, int> LookupIndexTable;

        public PropertyInfo PropInfo;

        public Type UnderlyingType;

        public bool IsParsableObject;

        public bool IsCollection;

        public string CollectionDelimiter;

        public Type CollectionUnderlyingType;
    }
}
