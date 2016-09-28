using DelimitedStringParser.Bond;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DelimitedStringParser.Tests
{
    [TestClass()]
    public class BondDelimitedStringParserTests
    {
        [TestMethod]
        public void BondTest1()
        {
            BondDelimitedStringParser<TestStruct1>.Parse("");
        }
    }
}
