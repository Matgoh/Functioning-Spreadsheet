using SpreadsheetUtilities;
using System.Data;

namespace FormulaTests
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      3 Feb 2023
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Matthew Goh - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Matthew Goh, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    /// 
    ///This is a test class for the formula class and is intended
    ///to contain all relevant formula class Unit Tests
    ///</summary>
    [TestClass]
    public class FormulaTests
    {
        // Test evaluate method with basic addition
        [TestMethod]
        public void TestSimpleAddition()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual((double)10, f.Evaluate(x => 6));
        }
        // Basic Subtraction
        [TestMethod]
        public void TestSimpleSubtractionInput()
        {
            Formula f = new Formula("5 -5");
            Formula g = new Formula("10 - 3 - 4 - 4");
            Formula h = new Formula("(10 + 3 - 4 - 2)");
            Assert.AreEqual((double)0, f.Evaluate(x => 6));
            Assert.AreEqual((double)-1, g.Evaluate(x => 6));
            Assert.AreEqual((double)7, h.Evaluate(x => 6));
        }

        // Testing order of operations
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

        // Testing placement of parenthesis
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
            Formula f = new Formula("-5 * 3");}

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception with closing parenthesis at the end")]
        public void TestFormulaExceptionPar()
        {
            Formula f = new Formula("(5) * 3)");}

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException),
        "Exception when operators as start of formula")]
        public void TestFormulaExceptionNumbers()
        {
            Formula f = new Formula("5(*12)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaExceptionPar2()
        {      
            Formula f = new Formula("()");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]       
        public void TestNumberPrev()
        {
            Formula f = new Formula("(3+5)9");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNumberPrev2()
        {
            Formula f = new Formula("3+5 9");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNumberPrev3()
        {
            Formula f = new Formula("3+A1 9");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("   ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarPrev()
        {
            Formula f = new Formula("9 + 10 / 19 + 4 + /");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarPrev2()
        {
            Formula f = new Formula("9 + 10 / 19 + 4 (+" );
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarInput()
        {
            Formula f = new Formula("9 + A1 * 5 / 2", x => x.ToUpper(), x => false);
            f.Evaluate(x => 6);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarInput2()
        {
            Formula f = new Formula("5 + 9 )A1 * 5 / 2");
            f.Evaluate(x => 6);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarInput3()
        {
            Formula f = new Formula("5 + 9 A1 * 5 / 2");
            f.Evaluate(x => 6);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVarInput4()
        {
            Formula f = new Formula("5 + 9 B4 A1 * 5 / 2");
            f.Evaluate(x => 6);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNormalize()
        {
            Formula formula_tester = new Formula("A1 + 3 * 4", x => "7", x => true);
        }

        // divide by 0 errors
        [TestMethod]
        public void TestDivideBy0()
        {
            Formula f = new Formula("10/0");
            Formula g = new Formula("10 + 19 *3 /0");
            Formula h = new Formula("10 + 19 *3 /A3");
            Formula i = new Formula("10 + (19 + 3) /0");
            Formula j = new Formula("(10 + 19) + (3 / 0)");
            Formula k = new Formula("10 / 0 + 19 + 3 /0");
            Assert.IsInstanceOfType(f.Evaluate(x => 6), typeof(FormulaError));
            Assert.IsInstanceOfType(g.Evaluate(x => 0), typeof(FormulaError));
            Assert.IsInstanceOfType(h.Evaluate(x => 0), typeof(FormulaError));
            Assert.IsInstanceOfType(i.Evaluate(x => 0), typeof(FormulaError));
            Assert.IsInstanceOfType(j.Evaluate(x => 0), typeof(FormulaError));
            Assert.IsInstanceOfType(k.Evaluate(x => 0), typeof(FormulaError));
        }
        
        // Test valid variable division
        [TestMethod]
        public void TestValidVarDivision()
        {
            Formula f = new Formula("10 + 19 *3 /A1");
            Assert.AreEqual(f.Evaluate(x => 3), (double)29);
        }

        // Test invalid variable
        [TestMethod]
        public void TestInvalidVariable()
        {
            Formula f = new Formula("10+A1");
            Assert.IsInstanceOfType(f.Evaluate(x => throw new ArgumentException()), typeof(FormulaError));
        }

        // Test invalid characters
        [TestMethod]
        public void TestInvalidCharacters()
        {
            Formula f = new Formula("^&$");
            Assert.IsInstanceOfType(f.Evaluate(x => throw new ArgumentException()), typeof(FormulaError));
        }

        // Test valid variables
        [TestMethod]
        public void TestValidVariable()
        {
            Formula f = new Formula("10+A1");
            Assert.AreEqual(f.Evaluate(x => 4), (double) 14);
        }

        // Test get variables
        [TestMethod]
        public void TestGetVariable()
        {
            Formula f = new Formula("10+a1 + b3 * c9", x => x.ToUpper(), x => true);
            Assert.AreEqual(3, f.GetVariables().Count());
        }

        // Test toString and the normalizer
        [TestMethod()]
        public void ToStringTest()
        {
            Formula toStringTest = new Formula("A1 * b1", x => x.ToUpper(), x => true);
            string str = toStringTest.ToString();
            Assert.AreEqual("A1*B1", str);
        }

        // Test Override bool false equals
        [TestMethod()]
        public void FalseEqualsTest()
        {
            object obj = new object();
            obj = null;

            object obj2 = new object();
            obj2 = "string";

            object obj3 = new Formula("A1 + 6 - 6 * b1", x => x.ToUpper(), x => true);

            object obj4 = new Formula("A1 - B4 * b1", x => x.ToUpper(), x => true);

            Formula test = new Formula("A1 + 9 - 6 * b1", x => x.ToUpper(), x => true);          
            Assert.IsFalse(test.Equals(obj));
            Assert.IsFalse(test.Equals(obj2));
            Assert.IsFalse(test.Equals(obj3));
            Assert.IsFalse(test.Equals(obj4));
        }

        // Test Override bool true equals
        [TestMethod()]
        public void TrueEqualsTest()
        {
            object obj = new Formula("A1 * b1", x => x.ToUpper(), x => true);

            Formula test = new Formula("A1 * b1", x => x.ToUpper(), x => true);
            Assert.IsTrue(test.Equals(obj));
        }

        // Test GetHashCode, should be the same for equal formulas
        [TestMethod()]
        public void TestGetHashCode()
        {
            Formula f2 = new Formula("A1 + b1", x => x.ToUpper(), x => true);
            String f1 = "A1+B1";
            int hash = f1.GetHashCode();
            int hash2 = f2.GetHashCode();
            Assert.AreEqual(hash, hash2);
        }

        // Test bool operator == and !=
        [TestMethod()]
        public void Testbool()
        {
            Formula f1 = new Formula("A1 + 9", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("A1 + 11", x => x.ToUpper(), x => true);
            Formula f3 = new Formula("A1 + 9", x => x.ToUpper(), x => true);

            Assert.IsTrue(f2 != f1);
            Assert.IsFalse(f3 != f1);
            Assert.IsTrue(f3 == f1);
            Assert.IsFalse(f1 == f2);
        }
    }
}