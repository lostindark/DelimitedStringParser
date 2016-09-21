using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelimitedStringParser
{
    public class DelimitedStringParser<T>
    {
        private static readonly string delimiter;
        private static readonly bool isVersionedString = false;

        /// <summary>
        /// Static constructor.
        /// It check the schema of the object and generate field parser.
        /// </summary>
        static DelimitedStringParser()
        {

        }

        /// <summary>
        /// Parse the string as object.
        /// It will return null if the input string is null, and will return default new instance of T if the input string is empty.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>The object</returns>
        public static T Parse(string str)
        {
            throw new NotImplementedException();
        }
    }
}
