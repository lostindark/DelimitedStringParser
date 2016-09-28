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

        public PropertyInfo PropInfo;

        public bool IsParsableObject;

        public bool IsCollection;

        public string CollectionDelimiter;

        public Type CollectionUnderlyingType;

        public int LookupIndex;

        public Dictionary<int, int> LookupIndexTable;
    }
}
