using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace SS
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
    /// Represents a spreadsheet class that has cells as its components. This class supports setting
    /// cell content, as a double, string, or formula, and accessing specified cells. The class throws
    /// a series of exceptions if the input does not support the criteria or results in a circular loop. 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Class to create a cell object
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// Contents can either be a string, double, or a formula. 
            /// </summary>
            public object Contents { get; set; }

            /// <summary>
            /// Added for AS5, this will store the value of a cell
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// Constructor if the content is a string
            /// </summary>
            /// <param name="name"></param>
            /// <param name="contents"></param>
            public Cell(string content)
            {
                Contents = content;
                Value = content;
            }

            /// <summary>
            /// Constructor if content is a double
            /// </summary>
            /// <param name="content"></param>
            public Cell(double content)
            {
                Contents = content;
                Value = content;
            }

            /// <summary>
            /// Constructor if the content is a formula
            /// </summary>
            /// <param name="content"></param>
            /// <param name="lookup"></param>
            public Cell(Formula content, Func<string, double> lookup)
            {
                Contents = content;
                Value = content.Evaluate(lookup);
            }
        }
        // Create field to store the names and cell object
        private Dictionary<String, Cell> cellSheet;

        // Create field for dependency data 
        private DependencyGraph graph;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Zero argument spreadsheet constructor
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cellSheet = new Dictionary<String, Cell>();
            graph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Three argument spreadsheet constructor
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
                           : base(isValid, normalize, version)
        {
            cellSheet = new Dictionary<String, Cell>();
            graph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Four argument spreadsheet constructor with the addition of a filepath constructor
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version)
                          : base(isValid, normalize, version)
        {
            cellSheet = new Dictionary<String, Cell>();
            graph = new DependencyGraph();
            Changed = false;

            try
            {
                if (!(GetSavedVersion(filePath).Equals(version)))
                {
                    throw new SpreadsheetReadWriteException("Versions do not match");
                }
                else
                    GetEntireFile(filePath);
            }
            catch (IOException)
            {
                throw new SpreadsheetReadWriteException("Cannot find file path");
            }
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is null or invalid
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public override object GetCellContents(string name)
        {
            // Throw if the name is null or invalid
            if (object.Equals(name, null) || !validName(name))
            {
                throw new InvalidNameException();
            }
            name = Normalize(name);
            if (cellSheet.ContainsKey(name))
            {
                return cellSheet[name].Contents;
            }
            else
                return "";
        }


        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Any empty cells should be removed from cellsheet.
            return cellSheet.Keys;
        }


        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (object.Equals(name, null) || !validName(name))
            {
                throw new InvalidNameException();
            }

            Cell cell = new Cell(number);
            if (cellSheet.ContainsKey(name))
            {
                cellSheet[name] = cell;
            }
            else
                cellSheet.Add(name, cell);

            graph.ReplaceDependees(name, new HashSet<String>());
            // All dependents would be the cells that need to recalculate after changing "name."
            List<string> nameAndDependees = new List<string>(GetCellsToRecalculate(name));

            return nameAndDependees;
        }


        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            graph.ReplaceDependees(name, new HashSet<String>());
            // All dependents would be the cells that need to recalculate after changing "name."
            List<string> nameAndDependees = new List<string>(GetCellsToRecalculate(name));
            // Do not add a cell if the content is empty
            if (text == "")
            {
                return nameAndDependees;
            }

            Cell cell = new Cell(text);
            if (cellSheet.ContainsKey(name))
            {
                cellSheet[name] = cell;
            }
            else
                cellSheet.Add(name, cell);

            return nameAndDependees;
        }


        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (string.Equals(name, null) || !validName(name))
            {
                throw new InvalidNameException();
            }

            if (object.Equals(formula, null))
            {
                throw new ArgumentNullException();
            }

            // Create a list to store dependees before replacement
            IEnumerable<String> oldDependees = graph.GetDependees(name);
            graph.ReplaceDependees(name, formula.GetVariables());

            try
            {
                List<String> nameAndDependees = new List<String>(GetCellsToRecalculate(name));
                // Create new cell that holds the value of the input formula
                Cell cell = new Cell(formula, GetVal);

                if (cellSheet.ContainsKey(name))
                    cellSheet[name].Contents = formula;
                else
                    cellSheet.Add(name, cell);

                return nameAndDependees;
            }
            catch (CircularException)
            {
                // If exception is caught, make sure old dependees still exist in graph
                graph.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }
        }


        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If the name is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependents(name);
        }

        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // List to hold all dependents to return
            List<String> dependents = new List<string>();
            string stringFormula;

            try
            {
                if (object.Equals(name, null) || !validName(name) || !IsValid(name))
                    throw new InvalidNameException();

                if (double.TryParse(content, out double value))
                {
                    // Set cell contents should already return list of dependents
                    dependents = new List<string>(SetCellContents(name, value));
                }

                // If there is no input, remove cell
                else if (content.Equals(""))
                {
                    cellSheet.Remove(name); return dependents;
                }
                // If content begins with an "="
                else if (content.Substring(0, 1).Equals("="))
                {
                    try
                    {
                        // Get formula without the equals sign
                        stringFormula = content.Substring(1, content.Length - 1);

                        // This will throw formula format exception if invalid
                        Formula f = new Formula(stringFormula, Normalize, IsValid);

                        // This will throw circular exception if invalid
                        dependents = new List<string>(SetCellContents(name, f));
                    }
                    catch
                    {
                        throw new FormulaFormatException("Formula error");
                    }
                }
                // Treat as a string
                else
                {
                    dependents = new List<string>(SetCellContents(name, content));
                }
                Changed = true;

                // ReEvaluate each of the cells that are dependent on the newly set element
                foreach (string cell in dependents)
                {
                    if (cellSheet[cell].Contents is Formula)
                    {
                        Formula g = (Formula)cellSheet[cell].Contents;
                        cellSheet[cell].Value = g.Evaluate(GetVal);
                    }
                }
                return dependents;
            }
            catch(Exception)
            {
                throw new FormulaFormatException("formula error");
            }
        }

        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(string filename)
        {
            if (object.Equals(filename, null) || filename.Equals(""))
            {
                throw new SpreadsheetReadWriteException("unable to read null or empty file");
            }
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name.Equals("spreadsheet"))
                            {
                                return reader["version"]!;
                            }
                            else
                                throw new SpreadsheetReadWriteException("Could not find version in XML file");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Issues with reading the file or looking up version information");
            }
            throw new SpreadsheetReadWriteException("Error with reading the XML file");
        }

        /// <summary>
        /// Similar functionality to getting the saved version, the only difference being that this
        /// method continues past the version, throughout the entire file, setting the name and content
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public void GetEntireFile(string filename)
        {
            if (object.Equals(filename, null) || filename.Equals(""))
            {
                throw new SpreadsheetReadWriteException("unable to read null or empty file");
            }
            try
            {
                // Initialize name and content of cell
                string name = "";
                string content = "";

                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name.Equals("spreadsheet"))
                            {
                                Version = reader["version"]!;
                            }
                            else if (reader.Name.Equals("name"))
                            {
                                reader.Read();
                                name = reader.Value;
                            }
                            else if (reader.Name.Equals("contents"))
                            {
                                reader.Read();
                                content = reader.Value;
                                SetContentsOfCell(name, content);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Obtaining contents failed");
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    string contents = "";
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    // This will write "version" equal to the set version value
                    writer.WriteAttributeString("version", Version);

                    foreach (string cell in cellSheet.Keys)
                    {
                        writer.WriteStartElement("cell");

                        // Writes the cell right after the name label
                        writer.WriteElementString("name", cell);

                        if (cellSheet[cell].Contents is double)
                        {
                            contents = cellSheet[cell].Contents.ToString()!;
                        }

                        if (cellSheet[cell].Contents is Formula)
                        {
                            contents = "=" + cellSheet[cell].Contents.ToString()!;
                        }

                        if (cellSheet[cell].Contents is string)
                        {
                            contents = (string)cellSheet[cell].Contents;
                        }

                        writer.WriteElementString("contents", contents);

                        // Close cell label
                        writer.WriteEndElement();
                    }
                    // close spreadsheet label and end document
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    Changed = false;
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("XML file conversion failed");
            }
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public override object GetCellValue(string name)
        {
            if (string.Equals(name, null) || !validName(name))
            {
                throw new InvalidNameException();
            }

            try
            {
                if (cellSheet.ContainsKey(name))
                {
                    return cellSheet[name].Value;
                }
                else
                    return "";
            }
            catch(Exception)
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Small helper method to determine if a cell name is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns> boolean of valid or not </returns>
        private bool validName(string name)
        {
            string strRegex = @"^[a-zA-Z]+[\d]+$";
            Regex regexPattern = new Regex(strRegex);

            if (regexPattern.IsMatch(name))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This helper method returns a double value, used for input for the formula method
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private double GetVal(string s)
        {
            if (GetCellValue(s) is double)
            {
                return (double)GetCellValue(s);
            }
            else
                throw new ArgumentException();
        }
    }
}
