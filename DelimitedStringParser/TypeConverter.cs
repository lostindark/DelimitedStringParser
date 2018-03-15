using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DelimitedStringParser
{
    /// <summary>
    /// Type converter used by DelimitedStringParser to convert string into target field types.
    /// </summary>
    public static class TypeConverter
    {
        enum ParseState
        {
            BeforeField,
            InField,
        }

        public static MethodInfo GetConvertMethod(Type type)
        {
            return typeof(TypeConverter).GetMethods().Single(m =>
                m.Name.StartsWith("ConvertTo")
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(string)
                && m.ReturnType == type);
        }

        public static MethodInfo GetConvertToIEnumerableMethod(Type type)
        {
            Type listUnderlyingType = type.GetGenericArguments()[0];
            return typeof(TypeConverter).GetMethod("ConvertToIEnumerable").MakeGenericMethod(listUnderlyingType);
        }

        public static IEnumerable<T> ConvertToIEnumerable<T>(string str, char delimiter, char? quote, char? escape, Func<string, T> convert)
        {
            return SplitField(str, delimiter, quote, escape)
                .Select(i => convert(i));
        }

        public static IEnumerable<string> SplitField(string str, char delimiter, char? quote, char? escape)
        {
            if (quote != null && delimiter == quote.Value)
            {
                throw new ArgumentException("The quote parameter can not be the same as the delimiter parameter.");
            }

            if (escape != null && delimiter == escape.Value)
            {
                throw new ArgumentException("The escape parameter can not be the same as the delimiter parameter.");
            }

            // If escape character is null, the quote character will be used.
            if (escape == null)
            {
                escape = quote;
            }

            StringBuilder value = new StringBuilder();

            if (!string.IsNullOrEmpty(str))
            {
                bool isEscaped = false;
                bool isQuoted = false;
                ParseState state = ParseState.BeforeField;

                for (int i = 0; i < str.Length; i++)
                {
                    char chr = str[i];

                    switch (state)
                    {
                        case ParseState.BeforeField:
                            if (chr == delimiter)
                            {
                                yield return value.ToString();
                                value.Clear();
                                isEscaped = false;
                                isQuoted = false;
                            }
                            else if (quote != null && chr == quote)
                            {
                                isQuoted = true;
                                state = ParseState.InField;
                            }
                            else if (escape != null && chr == escape)
                            {
                                isEscaped = true;
                                state = ParseState.InField;
                            }
                            else
                            {
                                state = ParseState.InField;
                                value.Append(chr);
                            }

                            break;

                        case ParseState.InField:
                            if (isEscaped)
                            {
                                // If quote character and escape character are the same, we treated the quote character as escaped already. We need to fix it.
                                if (escape == quote && chr == delimiter)
                                {
                                    yield return value.ToString();

                                    state = ParseState.BeforeField;
                                    value.Clear();
                                    isEscaped = false;
                                    isQuoted = false;
                                }
                                else
                                {
                                    value.Append(chr);
                                    isEscaped = false;
                                }
                            }
                            else if (isQuoted)
                            {
                                if (escape != null && chr == escape)
                                {
                                    isEscaped = true;
                                }
                                else if (chr == quote)
                                {
                                    isQuoted = false;
                                }
                                else
                                {
                                    value.Append(chr);
                                }
                            }
                            else if (escape != null && chr == escape)
                            {
                                isEscaped = true;
                            }
                            else if (chr == delimiter)
                            {
                                yield return value.ToString();

                                state = ParseState.BeforeField;
                                value.Clear();
                                isEscaped = false;
                                isQuoted = false;
                            }
                            else
                            {
                                value.Append(chr);
                            }

                            break;

                        default:
                            break;
                    }
                }
            }
                
            yield return value.ToString();
        }

        public static string ConvertToString(string str)
        {
            // TODO: to improve performance further, skip the convert method call for string type.
            return str;
        }

        public static bool ConvertToBool(string str)
        {
            const string FormatExceptionMessage = "String was not recognized as a valid Boolean.";

            if (string.IsNullOrEmpty(str))
            {
                throw new FormatException(FormatExceptionMessage);
            }

            str = str.Trim();

            if (str.Equals("0", StringComparison.OrdinalIgnoreCase)
                || str.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else if (str.Equals("1", StringComparison.OrdinalIgnoreCase)
                || str.Equals("-1", StringComparison.OrdinalIgnoreCase)
                || str.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            throw new FormatException(FormatExceptionMessage);
        }

        public static sbyte ConvertToInt8(string str)
        {
            return sbyte.Parse(str);
        }

        public static short ConvertToInt16(string str)
        {
            return short.Parse(str);
        }

        public static int ConvertToInt32(string str)
        {
            return int.Parse(str);
        }

        public static long ConvertToInt64(string str)
        {
            return long.Parse(str);
        }

        public static byte ConvertToUInt8(string str)
        {
            return byte.Parse(str);
        }

        public static ushort ConvertToUInt16(string str)
        {
            return ushort.Parse(str);
        }

        public static uint ConvertToUInt32(string str)
        {
            return uint.Parse(str);
        }

        public static ulong ConvertToUInt64(string str)
        {
            return ulong.Parse(str);
        }

        public static float ConvertToSingle(string str)
        {
            return float.Parse(str);
        }

        public static double ConvertToDouble(string str)
        {
            return double.Parse(str);
        }
    }
}
