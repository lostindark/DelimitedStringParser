using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace DelimitedStringParser
{
    /// <summary>
    /// A parser to parse delimited string into an object. It supports versioned or non-versioned format.
    /// </summary>
    /// <typeparam name="T">The target object type.</typeparam>
    /// <typeparam name="TClassMetadataReader">A class metadata reader that understand how to map the delimited string into the target class.</typeparam>
    /// <remarks>
    /// This parser supports all simple types, nullable, and list/set. Map is not supported yet.
    /// It requires custom attributes on the class definition.
    /// <para>
    /// For class, there are 2 attributes:
    /// 1. IsVersionedData. Default to false.
    /// 2. Delimiter. Specific what is used to seperate the fields in the string.
    /// </para>
    /// <para>
    /// For fields/properties, there are 2 attributes:
    /// 1. FieldId. Specific the field index in the string. The value can be a number, or a complex format to specify different field id for different version.
    /// 2. Delimiter. This is only valid for list/set fields/properties.
    /// </para>
    /// </remarks>
    public class DelimitedStringParser<T, TClassMetadataReader>
        where T : new()
        where TClassMetadataReader : IClassMetadataReader<T>, new()
    {
        private static readonly bool isVersionedData = false;
        private static readonly string delimiter;

        private static readonly List<FieldData> fieldList;

        /// <summary>
        /// Static constructor.
        /// It check the schema of the object and generate field parser.
        /// </summary>
        static DelimitedStringParser()
        {
            TClassMetadataReader classMetadataReader = new TClassMetadataReader();
            isVersionedData = classMetadataReader.IsVersionedData;
            delimiter = classMetadataReader.Delimiter;

            if (string.IsNullOrEmpty(delimiter))
            {
                throw new ArgumentException("The delimiter parameter cannot be null or empty.", nameof(delimiter));
            }

            fieldList = new List<FieldData>();
            foreach (var fieldMetadata in classMetadataReader.FieldList)
            {
                fieldList.Add(new FieldData()
                {
                    Metadata = fieldMetadata,
                    Parser = GenerateFieldPaser(fieldMetadata)
                });
            }
        }

        /// <summary>
        /// Parse the string as object.
        /// It will return default(T) if the input string is null, and will return default new instance of T if the input string is empty.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>The object</returns>
        public static T Parse(string str)
        {
            if (str == null)
            {
                return default(T);
            }

            T returnObject = new T();

            if (str.Length > 0)
            {
                int currentVersion = 0;
                string[] resultArray = str.Split(new string[] { delimiter }, StringSplitOptions.None);

                if (isVersionedData)
                {
                    // For versioned data, the first element is always version number.
                    //
                    currentVersion = int.Parse(resultArray[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
                }

                foreach (var field in fieldList)
                {
                    // Get field index in the delimited string.
                    //
                    int? fieldLookupIndex = GetLookupIndex(field.Metadata, isVersionedData, currentVersion);

                    if (fieldLookupIndex != null)
                    {
                        string valueStr = null;
                        if (fieldLookupIndex < resultArray.Length)
                        {
                            valueStr = resultArray[fieldLookupIndex.Value];
                        }

                        if (!string.IsNullOrEmpty(valueStr))
                        {
                            try
                            {
                                field.Parser(returnObject, valueStr);
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException(string.Format("Failed to parse field {0} with value: {1}.", "", valueStr), ex);
                            }
                        }
                    }
                }
            }

            return returnObject;
        }

        /// <summary>
        /// Generate a delegate to parse string to the type of the field and set it.
        /// </summary>
        private static Action<T, string> GenerateFieldPaser(FieldMetadata fieldMetadata)
        {
            PropertyInfo propInfo = fieldMetadata.PropInfo;
            Type underlyingType = propInfo.PropertyType;

            // The input string parameter. This is used to pass in the string representation of the field.
            //
            var strParameter = Expression.Parameter(typeof(string), "str");

            Expression parsingExpression = null;
            Type nullableUnderlyingType = null;

            if (fieldMetadata.IsCollection)
            {
                bool removeEmptyEntries = true;
                Delegate convertDelegate = null;
                Type collectionUnderlyingType = fieldMetadata.CollectionUnderlyingType;

                // Create a delegate to convert the string to the underlying type of the collection.
                // The delegate will be used by ConvertToIEnumerable method.
                //
                if (fieldMetadata.IsParsableObject)
                {
                    // Create the delegate to convert string to the object (using Parse method).
                    //
                    convertDelegate = Delegate.CreateDelegate(
                        Expression.GetFuncType(typeof(string), collectionUnderlyingType),
                        GetParseMethodForParsableObject(collectionUnderlyingType));
                    removeEmptyEntries = false;
                }
                else if (collectionUnderlyingType.IsPrimitive
                    || collectionUnderlyingType == typeof(string))
                {
                    convertDelegate = Delegate.CreateDelegate(
                        Expression.GetFuncType(typeof(string), collectionUnderlyingType),
                        TypeConverter.GetConvertMethod(collectionUnderlyingType));
                }
                else if (collectionUnderlyingType.IsEnum)
                {
                    Type enumUnderlyingType = Enum.GetUnderlyingType(collectionUnderlyingType);

                    // Create the delegate to convert string Enum underlying type, and then to Enum.
                    //
                    convertDelegate = Expression.Lambda(
                        Expression.Convert(
                            Expression.Call(null, TypeConverter.GetConvertMethod(enumUnderlyingType), strParameter),
                            collectionUnderlyingType),
                        strParameter).Compile();
                }
                else
                {
                    throw new NotSupportedException(GetFieldTypeNotSupportedMessage(fieldMetadata.Info));
                }

                // Find the constructor which takes IEnumerable<T>.
                //
                var constructor = propInfo.PropertyType.GetConstructor(
                    new Type[] { typeof(IEnumerable<>).MakeGenericType(new Type[] { collectionUnderlyingType }) });

                if (constructor == null)
                {
                    throw new NotSupportedException(GetFieldTypeNotSupportedMessage(fieldMetadata.Info));
                }

                // Use the ConvertToEnumerable method to convert the string into IEnumerable<T>, and then use the constructor to generate the property.
                //
                parsingExpression = Expression.New(
                    constructor,
                    Expression.Call(
                        TypeConverter.GetConvertToIEnumerableMethod(propInfo.PropertyType),
                        strParameter,
                        Expression.Constant(fieldMetadata.CollectionDelimiter),
                        Expression.Constant(removeEmptyEntries),
                        Expression.Constant(convertDelegate)));
            }
            else if (underlyingType.IsPrimitive
                || underlyingType == typeof(string))
            {
                // For simple types, we just need to use the convert method to convert the string into its type.
                // TODO: to improve performance further, skip the convert method call for string type.
                //
                parsingExpression = Expression.Call(null, TypeConverter.GetConvertMethod(underlyingType), strParameter);
            }
            else if (underlyingType.IsEnum)
            {
                // For enum, we need to get the underlying type so we can use propery convert method.
                //
                Type enumUnderlyingType = Enum.GetUnderlyingType(underlyingType);

                // We need to use Expression.Convert to convert the result back to Enum type.
                //
                parsingExpression = Expression.Convert(
                    Expression.Call(null, TypeConverter.GetConvertMethod(enumUnderlyingType), strParameter),
                    underlyingType);
            }
            else if ((nullableUnderlyingType = Nullable.GetUnderlyingType(underlyingType)) != null)
            {
                // Handle nullable fields of the following types:
                // 1. Primitive types (bool, int etc)
                // 2. Enum
                //
                underlyingType = nullableUnderlyingType;

                if (underlyingType.IsEnum)
                {
                    // For enum, we need to get the underlying type so we can use propery convert method.
                    //
                    underlyingType = Enum.GetUnderlyingType(underlyingType);
                }

                if (!underlyingType.IsPrimitive)
                {
                    throw new NotSupportedException(GetFieldTypeNotSupportedMessage(fieldMetadata.Info));
                }

                // We need to use Expression.Convert to convert the result back to nullable type.
                //
                parsingExpression = Expression.Convert(
                    Expression.Call(null, TypeConverter.GetConvertMethod(underlyingType), strParameter),
                    propInfo.PropertyType);
            }
            else if (fieldMetadata.IsParsableObject)
            {
                // Handle support object fields by creating a new parser.
                //
                parsingExpression = Expression.Call(GetParseMethodForParsableObject(propInfo.PropertyType), strParameter);
            }
            else
            {
                throw new NotSupportedException(GetFieldTypeNotSupportedMessage(fieldMetadata.Info));
            }

            var propertyParameter = Expression.Parameter(propInfo.DeclaringType, propInfo.Name);
            var assignExpression = Expression.MakeMemberAccess(propertyParameter, propInfo);

            // Generate the lambda to parse the string and assign the property.
            //
            return Expression.Lambda<Action<T, string>>(
                Expression.Assign(assignExpression, parsingExpression),
                propertyParameter,
                strParameter).Compile();
        }

        private static MethodInfo GetParseMethodForParsableObject(Type objectType)
        {
            Type classMetadataReaderType = typeof(TClassMetadataReader).GetGenericTypeDefinition().MakeGenericType(new Type[] { objectType });
            return typeof(DelimitedStringParser<,>).MakeGenericType(new Type[] { objectType, classMetadataReaderType }).GetMethod("Parse");
        }

        private static string GetFieldTypeNotSupportedMessage(string fieldInfo)
        {
            return string.Format(
                "Field type of {0} is not supported in this parser.",
                fieldInfo);
        }

        /// <summary>
        /// Get lookup index for the field.
        /// </summary>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <param name="isVersionedData">Whether the data is versioned.</param>
        /// <param name="version">Version of the versioned data.</param>
        /// <returns>The index of the field.</returns>
        private static int? GetLookupIndex(FieldMetadata fieldMetadata, bool isVersionedData, int version)
        {
            int? index = null;

            if (isVersionedData
                && fieldMetadata.LookupIndexTable != null)
            {
                // If it is versioned data, and the index lookup table is not empty,
                // we need to use the lookup table to find field index.
                //
                int tempIndex = -1;
                if (fieldMetadata.LookupIndexTable.TryGetValue(version, out tempIndex))
                {
                    index = tempIndex;
                }
            }
            else
            {
                index = fieldMetadata.LookupIndex;
            }

            // Versioned data always has the version as the first field, the actual index for other fields are off by 1.
            //
            if (isVersionedData && index != null)
            {
                index++;
            }

            return index;
        }

        private class FieldData
        {
            public FieldMetadata Metadata { get; set; }

            public Action<T, string> Parser { get; set; }
        }
    }
}
