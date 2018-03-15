using System;

namespace DelimitedStringParser
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class QuoteAttribute : Attribute
    {
        private readonly char? quote;

        /// <summary>
        /// Gets the quote character.
        /// </summary>
        /// <returns>The quote character.</returns>
        public virtual char? Value
        {
            get
            {
                return this.quote;
            }
        }

        /// <summary>
        /// Initializes a new instance of the QuoteAttribute class using the quote character.
        /// </summary>
        /// <param name="quote">The quote character.</param>
        public QuoteAttribute(char? quote)
        {
            this.quote = quote;
        }

        /// <summary>
        /// Determines whether two QuoteAttribute instances are equal.
        /// </summary>
        /// <returns>true if the value of the given object is equal to that of the current object; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:DelimitedStringParser.QuoteAttribute" /> to test the value equality of.</param>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            QuoteAttribute QuoteAttribute = obj as QuoteAttribute;
            return QuoteAttribute != null
                && QuoteAttribute.Value == this.Value;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <see cref="T:DelimitedStringParser.QuoteAttribute" />.</returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
