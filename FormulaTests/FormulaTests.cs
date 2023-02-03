using SpreadsheetUtilities;
using System.Data;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        // Test evaluate method with basic operations
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
        public void TestOrderOfOperations2()
        {
            Formula f = new Formula("(13-9) / 2 *(5 +2)");
            Assert.AreEqual((double)14, f.Evaluate(x => 6));
        }

        [TestMethod]
        public void TestLegalParenthesis()
        {
            Formula f = new Formula("((13)-9 / (2) *5 +2)");
            Assert.AreEqual((double)-7.5, f.Evaluate(x => 6));
        }

        // Tests for all the exceptions related to the input
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaExceptionNegative()
        {
            Formula f = new Formula("-5 * 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception with closing parenthesis at the end")]
        public void TestFormulaExceptionPar()
        {
            Formula f = new Formula("(5) * 3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaExceptionPar2()
        {
            Formula f = new Formula("(5 * 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaExceptionPar3()
        {
            Formula f = new Formula(")5(3)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaExceptionPar4()
        {
            Formula f = new Formula("5(12)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
 "Exception when operators as start of formula")]
        public void TestFormulaExceptionNumbers()
        {
            Formula f = new Formula("5(*12)");
        }
    }
}