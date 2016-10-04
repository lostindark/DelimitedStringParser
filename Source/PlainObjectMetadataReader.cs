using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DelimitedStringParser
{
    public class PlainObjectMetadataReader<T> : IClassMetadataReader<T>
    {
        public bool IsVersionedData
        {
            get
            {
                return typeof(T).GetCustomAttribute<IsVersionedDataAttribute>()?.IsVersionedData ?? false;
            }
        }

        public string Delimiter
        {
            get
            {
                return typeof(T).GetCustomAttribute<DelimiterAttribute>()?.Delimiter;
            }
        }

        public IEnumerable<FieldMetadata> FieldList
        {
            get
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    int lookupIndex = -1;
                    Dictionary<int, int> lookupIndexTable = new Dictionary<int, int>();
                    bool isParsableObject = false;
                    Type collectionUnderlyingType = null;
                    string collectionDelimiter = null;

                    bool isCollection = property.PropertyType.IsGenericType
                        && property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

                    if (isCollection)
                    {
                        collectionDelimiter = property.GetCustomAttribute<DelimiterAttribute>()?.Delimiter;

                        collectionUnderlyingType = property.PropertyType.GetGenericArguments()[0];
                        isParsableObject = collectionUnderlyingType.GetCustomAttributes<DelimiterAttribute>().Any();
                    }
                    else
                    {
                        isParsableObject = property.PropertyType.GetCustomAttributes<DelimiterAttribute>().Any();
                    }

                    var fieldIds = property.GetCustomAttributes<FieldIdAttribute>();
                    foreach (var fieldId in fieldIds)
                    {
                        if (fieldId.Version == -1)
                        {
                            if (fieldId.Index != -1)
                            {
                                lookupIndex = fieldId.Index;
                            }
                        }
                        else
                        {
                            lookupIndexTable.Add(fieldId.Version, fieldId.Index);
                        }
                    }

                    FieldMetadata metadata = new FieldMetadata()
                    {
                        PropInfo = property,
                        IsParsableObject = isParsableObject,
                        IsCollection = isCollection,
                        CollectionDelimiter = collectionDelimiter,
                        CollectionUnderlyingType = collectionUnderlyingType,
                        LookupIndex = lookupIndex,
                        LookupIndexTable = lookupIndexTable,
                    };

                    yield return metadata;
                }
            }
        }
    }
}
