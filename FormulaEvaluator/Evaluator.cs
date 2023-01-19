using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      18 Jan 2022
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
    ///    [... and of course you should describe the contents of the 
    ///    file in broad terms here ...]
    /// </summary>
    public class Evaluator
    {
        public delegate int Lookup(String variable_name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        /// 
        public static int Evaluate (string expression, Lookup variableEvaluator) 
        {
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();

            for (int i = 0; i < substrings.Length; i++)
            {
                // If the value is an integer, 
                if (int.TryParse(substrings[i], out int value)) 
                { 
                    if (operatorStack.Peek() == "*" || operatorStack.Peek() == "/")
                    {
                        int op = valueStack.Pop();
                    }
                }
            }

        }

        public static int Math
    }
}