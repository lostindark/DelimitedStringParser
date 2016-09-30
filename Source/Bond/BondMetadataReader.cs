using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Bond;

namespace DelimitedStringParser.Bond
{
    public class BondMetadataReader<T> : IClassMetadataReader<T>
    {
        private const string FieldIdAttribute = "FieldId";
        private const char FieldIndexDelimiter = ';';
        private const char KeyValueDelimiter = '=';
        private const char FieldIdVersionPrefix1 = 'v';
        private const char FieldIdVersionPrefix2 = 'V';

        private IEnumerable<AttributeAttribute> bondAttributes = typeof(T).GetCustomAttributes<AttributeAttribute>(false);

        public bool IsVersionedData
        {
            get
            {
                return bondAttributes
                    .Where(attr => attr.Name == "IsVersionedData")
                    .Select(x => TypeConverter.ConvertToBool(x.Value))
                    .DefaultIfEmpty(false)
                    .First();
            }
        }

        public string Delimiter
        {
            get
            {
                return bondAttributes
                    .Where(attr => attr.Name == "Delimiter")
                    .Select(x => x.Value)
                    .DefaultIfEmpty(null)
                    .First();
            }
        }

        public IEnumerable<FieldMetadata> FieldList
        {
            get
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    int lookupIndex;
                    Dictionary<int, int> lookupIndexTable;
                    bool isParsableObject = false;
                    Type collectionUnderlyingType = null;
                    string collectionDelimiter = null;

                    bool isCollection = property.PropertyType.IsGenericType
                        && property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

                    if (isCollection)
                    {
                        collectionDelimiter = property
                            .GetCustomAttributes<AttributeAttribute>(false)
                            .Where(attr => attr.Name == "Delimiter")
                            .Select(x => x.Value)
                            .DefaultIfEmpty(null)
                            .First();

                        collectionUnderlyingType = property.PropertyType.GetGenericArguments()[0];
                        isParsableObject = collectionUnderlyingType.GetCustomAttributes<SchemaAttribute>().Any();
                    }
                    else
                    {
                        isParsableObject = property.PropertyType.GetCustomAttributes<SchemaAttribute>().Any();
                    }

                    ParseFieldIndex(property, this.IsVersionedData, out lookupIndex, out lookupIndexTable);

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

        /// <summary>
        /// Parse field index lookup table.
        /// If the FieldId attribute is defined, parse it. Or else fallback to Bond field Id.
        /// </summary>
        private static void ParseFieldIndex(PropertyInfo property, bool isVersionedData, out int lookupIndex, out Dictionary<int, int> lookupIndexTable)
        {
            lookupIndex = -1;
            lookupIndexTable = null;

            string fieldIdStr = string.Join(
                FieldIndexDelimiter.ToString(),
                property.GetCustomAttributes<AttributeAttribute>(false).Where(attr => attr.Name == "FieldId").Select(x => x.Value));

            if (!string.IsNullOrEmpty(fieldIdStr))
            {
                try
                {
                    if (isVersionedData && fieldIdStr.IndexOf(KeyValueDelimiter) != -1)
                    {
                        lookupIndexTable = new Dictionary<int, int>();

                        foreach (var mappingEntry in fieldIdStr.Split(new char[] { FieldIndexDelimiter }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] mapping = mappingEntry.Split(new char[] { KeyValueDelimiter }, 2);
                            lookupIndexTable.Add(
                                int.Parse(mapping[0].TrimStart(FieldIdVersionPrefix1, FieldIdVersionPrefix2).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture),
                                int.Parse(mapping[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        lookupIndex = int.Parse(fieldIdStr, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException
                        || ex is FormatException
                        || ex is OverflowException)
                    {
                        throw new NotSupportedException(string.Format("The {0} attribute format is not valid on field {1}.", FieldIdAttribute, ""), ex);
                    }

                    throw;
                }
            }

            if (lookupIndex == -1)
            {
                lookupIndex = property.GetCustomAttributes<IdAttribute>(false)
                    .Select(x => GetBondId(x))
                    .DefaultIfEmpty(-1)
                    .First();
            }
        }

        private static int GetBondId(IdAttribute idAttr)
        {
            PropertyInfo valuePropery = typeof(IdAttribute).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
            return (ushort)valuePropery.GetValue(idAttr, BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, null);
        }
    }
}
