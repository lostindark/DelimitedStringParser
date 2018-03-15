using System;

namespace DelimitedStringParser
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field)]
    public class EscapeAttribute : Attribute
    {
        private readonly char? escape;

        /// <summary>
        /// Gets the escape character.
        /// </summary>
        /// <returns>The escape character.</returns>
        public virtual char? Value
        {
            get
            {
                return this.escape;
            }
        }

        /// <summary>
        /// Initializes a new instance of the EscapeAttribute class using the escape character.
        /// </summary>
        /// <param name="escape">The escape character.</param>
        public EscapeAttribute(char? escape)
        {
            this.escape = escape;
        }

        /// <summary>
        /// Determines whether two EscapeAttribute instances are equal.
        /// </summary>
        /// <returns>true if the value of the given object is equal to that of the current object; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:DelimitedStringParser.EscapeAttribute" /> to test the value equality of.</param>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            EscapeAttribute EscapeAttribute = obj as EscapeAttribute;
            return EscapeAttribute != null
                && EscapeAttribute.escape == this.escape;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <see cref="T:DelimitedStringParser.EscapeAttribute" />.</returns>
        public override int GetHashCode()
        {
            return this.escape.GetHashCode();
        }
    }
}
