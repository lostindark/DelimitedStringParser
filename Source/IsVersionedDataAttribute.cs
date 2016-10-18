using System;

namespace DelimitedStringParser
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class IsVersionedDataAttribute : Attribute
    {
        private static readonly IsVersionedDataAttribute Default = new IsVersionedDataAttribute(false);

        private readonly bool isVersionedData;

        /// <summary>
        /// Gets the IsVersionedData.
        /// </summary>
        /// <returns>IsVersionedData.</returns>
        public virtual bool IsVersionedData
        {
            get
            {
                return this.isVersionedData;
            }
        }

        /// <summary>
        /// Initializes a new instance of the IsVersionedDataAttribute class.
        /// </summary>
        /// <param name="isVersionedData">IsVersionedData.</param>
        public IsVersionedDataAttribute(bool isVersionedData)
        {
            this.isVersionedData = isVersionedData;
        }

        /// <summary>
        /// Determines whether two IsVersionedDataAttribute instances are equal.
        /// </summary>
        /// <returns>true if the value of the given object is equal to that of the current object; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:DelimitedStringParser.IsVersionedDataAttribute" /> to test the value equality of.</param>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            IsVersionedDataAttribute IsVersionedDataAttribute = obj as IsVersionedDataAttribute;
            return IsVersionedDataAttribute != null
                && IsVersionedDataAttribute.IsVersionedData == this.IsVersionedData;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <see cref="T:DelimitedStringParser.IsVersionedDataAttribute" />.</returns>
        public override int GetHashCode()
        {
            return this.IsVersionedData.GetHashCode();
        }

        /// <summary>Determines if this attribute is the default.</summary>
        /// <returns>true if the attribute is the default value for this attribute class; otherwise, false.</returns>
        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }
    }
}
