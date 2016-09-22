﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DelimitedStringParser
{
    /// <summary>
    /// Type converter used by DelimitedStringParser to convert string into target field types.
    /// </summary>
    public static class TypeConverter
    {
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

        public static IEnumerable<T> ConvertToIEnumerable<T>(string str, string delimiter, bool removeEmptyEntries, Func<string, T> convert)
        {
            return str
                .Split(new string[] { delimiter }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None)
                .Select(i => convert(i));
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
