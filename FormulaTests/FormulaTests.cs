using SpreadsheetUtilities;
using System.Data;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestSimpleAddition()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual((double)10, f.Evaluate(x => 6));
        }

        [TestMethod]
        public void TestSimpleSubtractionInput()
        {
            Formula f = new Formula("5 -5");
            Assert.AreEqual((double)0, f.Evaluate(x => 6));
        }

        [TestMethod]
        public void TestOrderOfOperations()
        {
            Formula f = new Formula("13-9 / 2 *5 +2");
            Assert.AreEqual((double)-7.5, f.Evaluate(x => 6));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaException()
        {
            Formula f = new Formula("-5 * 3");
        }
    }
}