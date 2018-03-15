using System;

namespace DelimitedStringParser
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class FieldIdAttribute : Attribute
    {
        private readonly int version;
        private readonly int index;

        public virtual int Version
        {
            get
            {
                return this.version;
            }
        }

        public virtual int Index
        {
            get
            {
                return this.index;
            }
        }

        public FieldIdAttribute(int index)
        {
            this.version = -1;
            this.index = index;
        }

        public FieldIdAttribute(int version, int index)
        {
            this.version = version;
            this.index = index;
        }

        /// <summary>
        /// Determines whether two FieldIdAttribute instances are equal.
        /// </summary>
        /// <returns>true if the value of the given object is equal to that of the current object; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:DelimitedStringParser.FieldIdAttribute" /> to test the value equality of.</param>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            FieldIdAttribute fieldIdAttribute = obj as FieldIdAttribute;
            return fieldIdAttribute != null
                && fieldIdAttribute.Version != this.Version
                && fieldIdAttribute.Index == this.Index;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <see cref="T:DelimitedStringParser.FieldIdAttribute" />.</returns>
        public override int GetHashCode()
        {
            return new { this.Version, this.Index }.GetHashCode();
        }
    }
}
