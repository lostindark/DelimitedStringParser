using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelimitedStringParser;

namespace DelimitedStringParser.Tests
{
    public enum TestEnum
    {
        Value0 = 0,
        Value1 = 1,
        Value2 = 2,
    }

    [Delimiter(";")]
    public class TestClass1
    {
        [FieldId(0)]
        public bool Field0 { get; set; }

        [FieldId(1)]
        public sbyte Field1 { get; set; }

        [FieldId(2)]
        public short Field2 { get; set; }

        [FieldId(3)]
        public int Field3 { get; set; }

        [FieldId(4)]
        public long Field4 { get; set; }

        [FieldId(5)]
        public byte Field5 { get; set; }

        [FieldId(6)]
        public ushort Field6 { get; set; }

        [FieldId(7)]
        public uint Field7 { get; set; }

        [FieldId(8)]
        public ulong Field8 { get; set; }

        [FieldId(9)]
        public string Field9 { get; set; }

        [FieldId(10)]
        public TestEnum Field10 { get; set; }

        public override bool Equals(object obj)
        {
            TestClass1 other = obj as TestClass1;

            if (other == null)
            {
                return false;
            }

            return this.Field0 == other.Field0
                && this.Field1 == other.Field1
                && this.Field2 == other.Field2
                && this.Field3 == other.Field3
                && this.Field4 == other.Field4
                && this.Field5 == other.Field5
                && this.Field6 == other.Field6
                && this.Field7 == other.Field7
                && this.Field8 == other.Field8
                && this.Field9 == other.Field9
                && this.Field10 == other.Field10;
        }

        public override int GetHashCode()
        {
            return new { this.Field0, this.Field1, this.Field2, this.Field3, this.Field4, this.Field5, this.Field6, this.Field7, this.Field8, this.Field9, this.Field10 }.GetHashCode();
        }
    }

    [Delimiter(";")]
    public partial class TestClass2
    {
        [FieldId(0)]
        public bool? Field0 { get; set; }

        [FieldId(1)]
        public TestEnum? Field1 { get; set; }

        [Delimiter(":")]
        [FieldId(2)]
        public List<int> Field2 { get; set; }

        [Delimiter(":")]
        [FieldId(3)]
        public List<TestEnum> Field3 { get; set; }

        [Delimiter(":")]
        [FieldId(4)]
        public LinkedList<int> Field4 { get; set; }

        [Delimiter(":")]
        [FieldId(5)]
        public HashSet<int> Field5 { get; set; }

        public override bool Equals(object obj)
        {
            TestClass2 other = obj as TestClass2;

            if (other == null)
            {
                return false;
            }

            return this.Field0 == other.Field0
                && this.Field1 == other.Field1
                && this.Field2.SequenceEqual(other.Field2)
                && this.Field3.SequenceEqual(other.Field3)
                && this.Field4.SequenceEqual(other.Field4)
                && this.Field5.SequenceEqual(other.Field5);
        }

        public override int GetHashCode()
        {
            return new { this.Field0, this.Field1, this.Field2, this.Field3, this.Field4, this.Field5 }.GetHashCode();
        }
    }

    [Delimiter("#")]
    public partial class TestClass3
    {
        [FieldId(0)]
        public int Field0 { get; set; }

        [FieldId(1)]
        public string Field1 { get; set; }

        public override bool Equals(object obj)
        {
            TestClass3 other = obj as TestClass3;

            if (other == null)
            {
                return false;
            }

            return this.Field0 == other.Field0
                && this.Field1 == other.Field1;
        }

        public override int GetHashCode()
        {
            return new { this.Field0, this.Field1 }.GetHashCode();
        }
    }

    [Delimiter(";")]
    public partial class TestClass4
    {
        [FieldId(0)]
        public int Field0 { get; set; }

        [FieldId(1)]
        public TestClass3 Field1 { get; set; }

        [Delimiter(":")]
        [FieldId(2)]
        public List<TestClass3> Field2 { get; set; }

        public override bool Equals(object obj)
        {
            TestClass4 other = obj as TestClass4;

            if (other == null)
            {
                return false;
            }

            return this.Field0.Equals(other.Field0)
                && this.Field1.Equals(other.Field1)
                && this.Field2.SequenceEqual(other.Field2);
        }

        public override int GetHashCode()
        {
            return new { this.Field0, this.Field1, this.Field2 }.GetHashCode();
        }
    }

    [IsVersionedData(true)]
    [Delimiter("|")]
    public partial class TestClass5
    {
        [FieldId(0)]
        public int Field0 { get; set; }

        [FieldId(1, 1)]
        public string Field1 { get; set; }

        [FieldId(1, 2)]
        [FieldId(2, 1)]
        public bool Field2 { get; set; }

        public override bool Equals(object obj)
        {
            TestClass5 other = obj as TestClass5;

            if (other == null)
            {
                return false;
            }

            return this.Field0 == other.Field0
                && this.Field1 == other.Field1
                && this.Field2 == other.Field2;
        }

        public override int GetHashCode()
        {
            return new { this.Field0, this.Field1, this.Field2 }.GetHashCode();
        }
    }
}
