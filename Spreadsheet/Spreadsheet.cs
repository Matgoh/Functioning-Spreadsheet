using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            /// name of cell. Its first character is an underscore or a letter</item>
            /// its remaining characters (if any) are underscores and/or letters and/or digits
            /// </summary>
            public string Name { get; set; }
            
           /// <summary>
           /// Contents can either be a string, double, or a formula. 
           /// </summary>
           public object Contents { get; set; }

           /// <summary>
           /// Constructor to set the name and contents of a cell.
           /// </summary>
           /// <param name="name"></param>
           /// <param name="contents"></param>
           public Cell(string name, object contents) 
           { 
                Name = name;
                Contents = contents;
           }
        }
        // Create field to store the names and cell object
        private Dictionary<String, Cell> cellSheet;

        // Create field for dependency data 
        private DependencyGraph graph;

        /// <summary>
        /// Spreadsheet constructor
        /// </summary>
        public Spreadsheet() 
        {
            cellSheet= new Dictionary<String, Cell>();
            graph= new DependencyGraph();
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
        public override ISet<string> SetCellContents(string name, double number)
        {            
            if (object.Equals(name, null) || !validName(name))
            {
                throw new InvalidNameException();
            }

            if (cellSheet.ContainsKey(name))
            {
                cellSheet[name].Contents = number;
            }
            else 
                cellSheet.Add(name, new Cell(name, number));

            graph.ReplaceDependees(name, new HashSet<String>());
            // All dependents would be the cells that need to recalculate after changing "name."
            HashSet<string> nameAndDependees = new HashSet<string>(GetCellsToRecalculate(name));

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
        public override ISet<string> SetCellContents(string name, string text)
        {           
            if (object.Equals(name, null) || (!validName(name)))
            {
                throw new InvalidNameException();
            }

            if (object.Equals(text, null))
            {
                throw new ArgumentNullException();
            }

            graph.ReplaceDependees(name, new HashSet<String>());
            // All dependents would be the cells that need to recalculate after changing "name."
            HashSet<string> nameAndDependees = new HashSet<string>(GetCellsToRecalculate(name));
            // Do not add a cell if the content is empty
            if (text == "")
            {
                return nameAndDependees;
            }

            if (cellSheet.ContainsKey(name))
            {
                cellSheet[name].Contents = text;
            }
            else
                cellSheet.Add(name, new Cell(name, text));

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
        public override ISet<string> SetCellContents(string name, Formula formula)
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
                HashSet<String> nameAndDependees = new HashSet<String>(GetCellsToRecalculate(name));
                
                if (cellSheet.ContainsKey(name))    
                    cellSheet[name].Contents = formula;         
                else
                    cellSheet.Add(name, new Cell(name, formula));      

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
        /// Small helper method to determine if a cell name is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns> boolean of valid or not </returns>
        private bool validName (string name)
        {
            string strRegex = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
            Regex regexPattern = new Regex(strRegex);

            if (regexPattern.IsMatch(name))
            {
                return true;
            }
            else
                return false;
        }
    }
}
