using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DelimitedStringParser
{
    public class DelimitedStringParser<T, TClassMetadataReader>
        where T : new()
        where TClassMetadataReader : IClassMetadataReader<T>, new()
    {
        private static readonly bool isVersionedData = false;
        private static readonly string delimiter;

        private static readonly List<FieldData> fieldDataList;

        /// <summary>
        /// Static constructor.
        /// It check the schema of the object and generate field parser.
        /// </summary>
        static DelimitedStringParser()
        {
            TClassMetadataReader classMetadataReader = new TClassMetadataReader();
            isVersionedData = classMetadataReader.IsVersionedData;
            delimiter = classMetadataReader.Delimiter;
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

                foreach (var field in fieldDataList)
                {
                    // Get field index in the delimited string. For composite datapoint, we need to pass in the version.
                    //
                    int? fieldLookupIndex = field.GetLookupIndex(isVersionedData, currentVersion);

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
        /// Represent the field data of the target object.
        /// It builds the index lookup table and the parser for the field.
        /// </summary>
        private class FieldData
        {
            private int lookupIndex = -1;
            private Dictionary<int, int> lookupIndexTable = null;

            public Action<T, string> Parser { get; private set; }

            public FieldData()
            {
            }

            /// <summary>
            /// Get lookup index for the field.
            /// </summary>
            /// <param name="version">Version of the versioned data.</param>
            /// <returns>The index of the field.</returns>
            public int? GetLookupIndex(bool isVersionedData, int version)
            {
                int? index = null;

                if (isVersionedData
                    && this.lookupIndexTable != null)
                {
                    // If it is versioned data, and the index lookup table is not empty,
                    // We need to use the lookup table to find field index. Don't fall back to lookupIndex member.
                    //
                    int tempIndex = -1;
                    if (this.lookupIndexTable.TryGetValue(version, out tempIndex))
                    {
                        index = tempIndex;
                    }
                }
                else
                {
                    index = this.lookupIndex;
                }

                // Versioned data always has version as the first field, the actual index for other fields is off by 1.
                //
                if (isVersionedData && index != null)
                {
                    index++;
                }

                return index;
            }
        }

    }
}
