using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestFormulaCreate()
        {
            Assert.IsTrue(Formula f = new Formula("5+5"));
        }
    }
}