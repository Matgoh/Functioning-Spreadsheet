using SpreadsheetUtilities;
using SS;
using System.Security.Cryptography;

namespace SpreadsheetTests
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      10 Feb 2023
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
    ///This is a test class for the Spreadsheet class and is intended
    ///to contain all relevant spreadsheet class Unit Tests
    ///</summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Test get invalid name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (InvalidNameException))]
        public void Test1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 12.0);
            s.GetCellContents("3B");
        }

        /// <summary>
        /// Test get null content
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 12.0);
            s.GetCellContents(null);
        }

        /// <summary>
        /// Test invalid name for doubles
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("5G1", 12.0);
        }

        /// <summary>
        /// Test invalid name for strings
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("996s", "invalid");
        }

        /// <summary>
        /// Test invalid name for formulas
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = new Formula("A1 + B3", x => x.ToUpper(), x => true);
            s.SetCellContents("035s", f1);
        }

        /// <summary>
        /// Test null text for strings
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("Q5", (string) null);
        }

        /// <summary>
        /// Test null name for doubles
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 12.5);
        }

        /// <summary>
        /// Test null name for Formulas
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = new Formula("A1 + B3", x => x.ToUpper(), x => true);
            s.SetCellContents(null, f1);
        }

        /// <summary>
        /// Test null name for Strings
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            Spreadsheet s = new Spreadsheet();            
            s.SetCellContents(null, "it is null");
        }

        /// <summary>
        /// Test null Formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test10()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = null;
            s.SetCellContents("S2", f1);
        }

        /// <summary>
        /// Test get cell contents
        /// </summary>
        [TestMethod]
        public void Test11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 12.0);
            s.SetCellContents("A2", "Hello world");
            s.SetCellContents("A3", "A1 + B2");
            s.SetCellContents("B2", 4.0);
            Assert.AreEqual(12.0, s.GetCellContents("A1"));
            Assert.AreEqual("Hello world", s.GetCellContents("A2"));
            Assert.AreEqual("A1 + B2", s.GetCellContents("A3"));
            Assert.AreEqual("", s.GetCellContents("A9"));
        }

        /// <summary>
        /// Test get names of non empty cells
        /// </summary>
        [TestMethod]
        public void Test12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 12.0);
            s.SetCellContents("B1", "");
            s.SetCellContents("A2", "Hello world");
            s.SetCellContents("A3", "A1 + B2");
            s.SetCellContents("B2", 4.0);

            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A2"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A3"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("B2"));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("B3"));
        }

        /// <summary>
        /// Test set cell contents for doubles and strings
        /// </summary>
        [TestMethod]
        public void Test13()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsTrue(s.SetCellContents("A1", 12.0).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("B1", "").Contains("B1"));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("B1")); // Make sure "B1" is considered empty
            Assert.IsTrue(s.SetCellContents("A2", "Hello world").Contains("A2"));            
            Assert.IsTrue(s.SetCellContents("A3", "A1 + 5").Contains("A3"));
            Assert.IsTrue(s.SetCellContents("A1", 9.7).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A2", "Fine").Contains("A2"));
        }

        /// <summary>
        /// Test set cell content for formulas
        /// </summary>
        [TestMethod]
        public void Test14()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = new Formula("8 / 2", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("A1 * 12", x => x.ToUpper(), x => true);
            Formula f3 = new Formula("12", x => x.ToUpper(), x => true);
            Assert.IsTrue(s.SetCellContents("A1", f1).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A2", f2).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A2", 12.0).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A1", 5.0).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A1", f3).Contains("A1"));
        }

        /// <summary>
        /// Test Circular Exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = new Formula("8 / A2", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("A1 * 12", x => x.ToUpper(), x => true);
            Formula f3 = new Formula("12", x => x.ToUpper(), x => true);
            Assert.IsTrue(s.SetCellContents("A1", f1).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A2", f2).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A2", 12.0).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A1", 5.0).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A1", f3).Contains("A1"));
        }

        /// <summary>
        /// Test Circular Exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            Spreadsheet s = new Spreadsheet();
            Formula f1 = new Formula("8 / A2", x => x.ToUpper(), x => true);
            Formula f2 = new Formula("12 * A3", x => x.ToUpper(), x => true);
            Formula f3 = new Formula("A1", x => x.ToUpper(), x => true);
            Assert.IsTrue(s.SetCellContents("A1", f1).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A2", f2).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A3", f3).Contains("A2"));
            Assert.IsTrue(s.SetCellContents("A1", 5.0).Contains("A1"));
            Assert.IsTrue(s.SetCellContents("A1", f3).Contains("A1"));
        }
    }
}