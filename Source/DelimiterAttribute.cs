using System;

namespace DelimitedStringParser
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class DelimiterAttribute : Attribute
    {
        private static readonly DelimiterAttribute Default = new DelimiterAttribute(string.Empty);

        private readonly string delimiter;

        /// <summary>
        /// Gets the Delimiter.
        /// </summary>
        /// <returns>The delimiter.</returns>
        public virtual string Delimiter
        {
            get
            {
                return this.delimiter;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DelimiterAttribute class using the delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        public DelimiterAttribute(string delimiter)
        {
            this.delimiter = delimiter;
        }

        /// <summary>
        /// Determines whether two DelimiterAttribute instances are equal.
        /// </summary>
        /// <returns>true if the value of the given object is equal to that of the current object; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:DelimitedStringParser.DelimiterAttribute" /> to test the value equality of.</param>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DelimiterAttribute delimiterAttribute = obj as DelimiterAttribute;
            return delimiterAttribute != null
                && delimiterAttribute.Delimiter != null
                && delimiterAttribute.Delimiter == this.Delimiter;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <see cref="T:DelimitedStringParser.DelimiterAttribute" />.</returns>
        public override int GetHashCode()
        {
            return this.Delimiter.GetHashCode();
        }

        /// <summary>Determines if this attribute is the default.</summary>
        /// <returns>true if the attribute is the default value for this attribute class; otherwise, false.</returns>
        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }
    }
}
