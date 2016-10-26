using System;
using System.Collections.Generic;
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

        public char Delimiter
        {
            get
            {
                var delimiterAttribute = typeof(T).GetCustomAttribute<DelimiterAttribute>();
                if (delimiterAttribute == null)
                {
                    throw new ArgumentException(string.Format("You must specificy a delimiter for {0}.", nameof(T)));
                }

                return delimiterAttribute.Value;
            }
        }

        public char? Quote
        {
            get
            {
                return typeof(T).GetCustomAttribute<QuoteAttribute>()?.Value;
            }
        }

        public char? Escape
        {
            get
            {
                return typeof(T).GetCustomAttribute<EscapeAttribute>()?.Value;
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
                    char? collectionDelimiter = null;
                    char? collectionQuote = null;
                    char? collectionEscape = null;

                    bool isCollection = property.PropertyType.IsGenericType
                        && property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

                    if (isCollection)
                    {
                        collectionDelimiter = property.GetCustomAttribute<DelimiterAttribute>()?.Value;

                        if (collectionDelimiter == null)
                        {
                            throw new ArgumentException(string.Format("You must specificy a delimiter for {0} given it's collection.", property.Name));
                        }

                        collectionQuote = property.GetCustomAttribute<QuoteAttribute>()?.Value;
                        collectionEscape = property.GetCustomAttribute<EscapeAttribute>()?.Value;

                        collectionUnderlyingType = property.PropertyType.GetGenericArguments()[0];
                        isParsableObject = collectionUnderlyingType.GetCustomAttributes<DelimiterAttribute>().Any();
                    }
                    else
                    {
                        isParsableObject = property.PropertyType.GetCustomAttributes<DelimiterAttribute>().Any();
                    }

                    var fieldIds = property.GetCustomAttributes<FieldIdAttribute>();

                    if (fieldIds != null && fieldIds.Any())
                    {
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
                            CollectionQuote = collectionQuote,
                            CollectionEscape = collectionEscape,
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
}
