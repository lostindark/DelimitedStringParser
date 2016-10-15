using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DelimitedStringParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelimitedStringParser.Tests
{
    [TestClass()]
    public class DelimitedStringParserTests
    {
        [TestMethod()]
        public void ParserTestNullValue()
        {
            var testClass1 = PlainObjectDelimitedStringParser<TestClass1>.Parse(null);
            Assert.IsNull(testClass1);
        }

        [TestMethod()]
        public void ParserTestEmptyValue()
        {
            var testClass1 = PlainObjectDelimitedStringParser<TestClass1>.Parse("");
            Assert.AreEqual(new TestClass1(), testClass1);
        }

        [TestMethod()]
        public void ParserTestClassEmptyForAllFields()
        {
            var expected = new TestClass1();
            var actual = PlainObjectDelimitedStringParser<TestClass1>.Parse(";;;;;;;;;;");

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test simple types and enum.
        /// </summary>
        [TestMethod()]
        public void ParserTestSimpleTypesAndEnum()
        {
            var expected = new TestClass1
            {
                Field0 = true,
                Field1 = sbyte.MaxValue,
                Field2 = Int16.MaxValue,
                Field3 = Int32.MaxValue,
                Field4 = Int64.MaxValue,
                Field5 = byte.MaxValue,
                Field6 = UInt16.MaxValue,
                Field7 = UInt32.MaxValue,
                Field8 = UInt64.MaxValue,
                Field9 = "str",
                Field10 = TestEnum.Value1,
            };

            var str = string.Format(
                "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
                expected.Field0 ? 1 : 0,
                expected.Field1,
                expected.Field2,
                expected.Field3,
                expected.Field4,
                expected.Field5,
                expected.Field6,
                expected.Field7,
                expected.Field8,
                expected.Field9,
                (int)expected.Field10);

            var actual = PlainObjectDelimitedStringParser<TestClass1>.Parse(str);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test nullable, vector/list/set.
        /// </summary>
        [TestMethod()]
        public void ParserTestNullableAndCollections()
        {
            var expected = new TestClass2
            {
                Field0 = true,
                Field1 = TestEnum.Value2,
                Field2 = new List<int>() { 1, 2, 3 },
                Field3 = new List<TestEnum>() { TestEnum.Value1, TestEnum.Value0 },
                Field4 = new LinkedList<int>(new int[] { 1, 2, 3 }),
                Field5 = new HashSet<int>() { 1, 2, 3 },
            };

            var str = string.Format(
                "{0};{1};{2};{3};{4};{5}",
                expected.Field0 == null ? "" : (expected.Field0.Value ? "1" : "0"),
                (int)expected.Field1,
                string.Join(":", expected.Field2),
                string.Join(":", expected.Field3.Select(o => (int)o)),
                string.Join(":", expected.Field4),
                string.Join(":", expected.Field5));

            var actual = PlainObjectDelimitedStringParser<TestClass2>.Parse(str);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test nested class.
        /// </summary>
        [TestMethod()]
        public void ParserTestNestedClass()
        {
            var expected = new TestClass4
            {
                Field0 = 2,
                Field1 = new TestClass3
                {
                    Field0 = 3,
                    Field1 = "str1"
                },
                Field2 = new List<TestClass3>()
                {
                    new TestClass3
                    {
                        Field0 = 4,
                        Field1 = "str2"
                    },
                    new TestClass3
                    {
                        Field0 = 5,
                        Field1 = "str3"
                    }
                },
            };

            var str = string.Format(
                "{0};{1};{2}",
                expected.Field0,
                string.Format("{0}#{1}", expected.Field1.Field0, expected.Field1.Field1),
                string.Join(":", expected.Field2.Select(f => string.Format("{0}#{1}", f.Field0, f.Field1))));

            var actual = PlainObjectDelimitedStringParser<TestClass4>.Parse(str);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test Versioned Data
        /// </summary>
        [TestMethod()]
        public void ParserTestVersionedData()
        {
            // v1
            var expected = new TestClass5
            {
                Field0 = 2,
                Field1 = "abc",
                Field2 = true,
            };

            var actual = PlainObjectDelimitedStringParser<TestClass5>.Parse("1|2|abc|1");
            Assert.AreEqual(expected, actual);

            // v2
            expected = new TestClass5
            {
                Field0 = 2,
                Field2 = false,
            };

            actual = PlainObjectDelimitedStringParser<TestClass5>.Parse("2|2|0");
            Assert.AreEqual(expected, actual);
        }
    }
}
