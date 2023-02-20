using SpreadsheetUtilities;
using SS;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace SpreadsheetTests
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      20 Feb 2023
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
            s.SetContentsOfCell("A1", "12.0");
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
            s.SetContentsOfCell("A1", "12.0");
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
            s.SetContentsOfCell("5G1", "12.0");
        }

        /// <summary>
        /// Test invalid name for strings
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("996s", "invalid");
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
            s.SetContentsOfCell("035s", "f1");
        }

        /// <summary>
        /// Test null name for doubles
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "12.5");
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
            s.SetContentsOfCell(null, "f1");
        }

        /// <summary>
        /// Test null name for Strings
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            Spreadsheet s = new Spreadsheet();            
            s.SetContentsOfCell(null, "it is null");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Test10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "= A3");
            s.SetContentsOfCell("A2", "= A1 + 9");
            s.SetContentsOfCell("A3", "= A2 + 5");
            s.SetContentsOfCell("B2", "= A3 - A1");
        }

            /// <summary>
            /// Test get cell contents
            /// </summary>
            [TestMethod]
        public void Test11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "12.0");
            s.SetContentsOfCell("A2", "Hello world");
            s.SetContentsOfCell("A3", "A1 + B2");
            s.SetContentsOfCell("B2", "4.0");
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
            s.SetContentsOfCell("A1", "12.0");
            s.SetContentsOfCell("B1", "");
            s.SetContentsOfCell("A2", "Hello world");
            s.SetContentsOfCell("A3", "A1 + B2");
            s.SetContentsOfCell("B2", "4.0");

            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A2"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("A3"));
            Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Contains("B2"));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("B1"));
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().Contains("B3"));
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
            Assert.IsTrue(s.SetContentsOfCell("A1", "f1").Contains("A1"));
            Assert.IsTrue(s.SetContentsOfCell("A2", "f2").Contains("A2"));
            Assert.IsTrue(s.SetContentsOfCell("A2", "12.0").Contains("A2"));
            Assert.IsTrue(s.SetContentsOfCell("A1", "5.0").Contains("A1"));
            Assert.IsTrue(s.SetContentsOfCell("A1", "f3").Contains("A1"));
        }

        // AS5 TESTS

        // Tests IsValid
        [TestMethod()]
        public void ValidTest1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A131", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void ValidTest2()
        {
            Spreadsheet s = new Spreadsheet(s => s[0] != 'A', s => s, "default");
            s.SetContentsOfCell("A131", "x");
        }

        [TestMethod()]
        public void NormalizeTest()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s.ToUpper(), "Default");
            Spreadsheet a = new Spreadsheet(s => true, s => s.ToUpper(), "Default");
            s.SetContentsOfCell("B1", "Matt");
            a.SetContentsOfCell("A1", "10.0");
            a.SetContentsOfCell("A2", "= A1");
            a.SetContentsOfCell("f1", "= A2");
            Assert.AreEqual("Matt", s.GetCellContents("b1"));
            Assert.AreEqual(a.GetCellValue("a1"), a.GetCellValue("F1"));
        }

        [TestMethod()]
        public void EmptyTest()
        {
            Spreadsheet s = new Spreadsheet();            
            s.SetContentsOfCell("B1", "");
            Assert.IsTrue(s.GetCellContents("B1").Equals(""));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("", "");          
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("0032r", "12.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyTest4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "6541.9");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void EmptyTest5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]

        public void EmptyTest7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save("hello.txt");
            s = new Spreadsheet("hello.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save("hello.txt");
            s = new Spreadsheet("nothere.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "12.5");
            s.Save("hello.txt");
            s = new Spreadsheet("hello.txt", s => true, s => s, "default");
            Assert.AreEqual(12.5, s.GetCellValue("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, "1.1");
            s.Save("hello.txt");
            s = new Spreadsheet("hello.txt", s => true, s => s, "1.2");
        }

        [TestMethod()]
        public void SaveTest5()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, "Version");
            s.Save("hi.txt");
            Assert.AreEqual("Version", new Spreadsheet().GetSavedVersion("hi.txt"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest6()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, "");
            s.Save("");           
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest7()
        {
            Spreadsheet s = new Spreadsheet("", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest8()
        {
            Spreadsheet s = new Spreadsheet("hi.txt", s => true, s => s, "Version");
            s.SetContentsOfCell("A5", "=K9");
            s.SetContentsOfCell("A2", "what");
            s.Save("hi.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest9()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, "Version");
            s.Save("hi.txt");
            s.GetSavedVersion("nonexistent.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest10()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetSavedVersion("nonexistent.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FileTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetEntireFile("nonexistent.txt");
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void CellValueTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "12");
            s.GetCellValue(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void CellValueTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "12");
            s.GetCellValue("23df");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FileTest2()
        {
            using (StreamWriter writer = new StreamWriter("helloworld.txt"))
            {
                writer.WriteLine("hello");
                writer.WriteLine("this");
                writer.WriteLine("should");
                writer.WriteLine("be");
                writer.WriteLine("test");
            }
            Spreadsheet s = new Spreadsheet("helloworld.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedTest()
        {
            using (StreamWriter writer = new StreamWriter("helloworld.txt"))
            {
                writer.WriteLine("hello");
                writer.WriteLine("this");
                writer.WriteLine("should");
                writer.WriteLine("be");
                writer.WriteLine("test");
            }
            Spreadsheet s = new Spreadsheet("helloworld.txt", s => true, s => s, "");
            s.GetSavedVersion("helloworld.txt");
        }
    }

    
}