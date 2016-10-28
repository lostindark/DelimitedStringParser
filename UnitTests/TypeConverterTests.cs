using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelimitedStringParser.Tests
{
    [TestClass()]
    public class TypeConverterTests
    {
        [TestMethod()]
        public void SplitFieldTestEmptyData()
        {
            var expected = new List<string> { "" };
            var actual = TypeConverter.SplitField("", ',', null, null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTest3EmptyField()
        {
            var expected = new List<string> { "", "", "" };
            var actual = TypeConverter.SplitField(",,", ',', null, null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestNoQuoteAndEscape()
        {
            var expected = new List<string> { "1997", "Ford", "E350" };
            var actual = TypeConverter.SplitField("1997,Ford,E350", ',', null, null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestQuotedField()
        {
            var expected = new List<string> { "1997", "Ford", "E350" };
            var actual = TypeConverter.SplitField("\"1997\",\"Ford\",\"E350\"", ',', '"', null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestQuotedFieldWithDelimiter()
        {
            var expected = new List<string> { "1997", "Ford", "E350", "Super, luxurious truck" };
            var actual = TypeConverter.SplitField("1997,Ford,E350,\"Super, luxurious truck\"", ',', '"', null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestQuotedFieldWithDelimiterInTheMiddle()
        {
            var expected = new List<string> { "1997", "Ford", "Super, luxurious truck", "E350" };
            var actual = TypeConverter.SplitField("1997,Ford,\"Super, luxurious truck\",E350", ',', '"', null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestQuotedFieldWithDelimiterAndQuote()
        {
            var expected = new List<string> { "1997", "Ford", "E350", "Super, \"luxurious\" truck" };
            var actual = TypeConverter.SplitField("1997,Ford,E350,\"Super, \"\"luxurious\"\" truck\"", ',', '"', null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestQuotedFieldWithQuotes()
        {
            var expected = new List<string> { "1997", "Ford", "E350", "", "\"", "\"\"" };
            var actual = TypeConverter.SplitField("1997,Ford,E350,\"\",\"\"\"\",\"\"\"\"\"\"", ',', '"', null).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void SplitFieldTestEscape()
        {
            var expected = new List<string> { "1997", "Ford", "E350", "Super, luxurious truck", "Test" };
            var actual = TypeConverter.SplitField("1997,Ford,E350,Super\\, luxurious truck,Test", ',', null, '\\').ToList();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
