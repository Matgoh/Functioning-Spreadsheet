using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Matthew Goh
    /// Partner:   None
    /// Date:      23 Jan 2023
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
    /// This file is a library class containing an "evaluate" method, for
    /// calculating the value of a given expression, and a "compute" 
    /// method, which assists in doing the mathematical computations.
    /// </summary>
    public class Evaluator
    {
        public delegate int Lookup(String variable_name);
        /// <summary>
        /// This evaluate method will calculate the integer value of an
        /// inputted expression
        /// </summary>
        /// <param name="expression">The inputted expression, accepting 
        /// numbers, variables, parentheses, "+", "-", "*" and "/" 
        /// operations</param>
        /// <param name="variableEvaluator">delegate function to 
        /// determine the value of the variable</param>
        /// <returns> The evaluated result of the inputted expression
        /// </returns>
        ///                                                                   
        public static int Evaluate(string expression, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Stack<int> valueStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();

            // Create regular expression outside the for loop
            string strRegex = @"[a-zA-z]+\d+";
            Regex var = new Regex(strRegex);

            for (int i = 0; i < substrings.Length; i++)
            {
                // If the value is an integer... 
                if (int.TryParse(substrings[i], out int integer))
                {
                    if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/") && valueStack.Count > 0)
                    {
                        valueStack.Push(Compute(valueStack.Pop(), integer, operatorStack.Pop()));
                    }
                    else
                    {
                        valueStack.Push(integer);
                    }
                }

                // If t is a variable...
                if (var.IsMatch(substrings[i]))
                {
                    // When operator stack contains "*" or "/" and the value stack is not empty, compute value on stack, variable, and
                    // operator and push to value stack
                    if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/") && valueStack.Count > 0)
                    {
                        valueStack.Push(Compute(valueStack.Pop(), variableEvaluator(substrings[i]), operatorStack.Pop()));
                    }

                    else
                    {
                        valueStack.Push(variableEvaluator(substrings[i]));
                    }
                }

                // If t is + or -...
                if (substrings[i] == "+" || substrings[i] == "-")
                {
                    // If operator stacks contain "+" or "-" and value stack is 2 or greater, pop operand and both values and push compute 
                    if (operatorStack.Count > 0 && valueStack.Count >= 2 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
                    {
                        int secondVal = valueStack.Pop();

                        valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                    }
                    operatorStack.Push(substrings[i]);
                }

                // If t is *, /, or (...
                if (substrings[i] == "*" || substrings[i] == "/" || substrings[i] == "(")
                {
                    operatorStack.Push(substrings[i]);
                }

                // If t is )...
                if (substrings[i] == ")")
                {
                    // If operator stacks contain "+" or "-" and value stack is 2 or greater, pop operand and both values and push compute 
                    if (operatorStack.Count() > 0 && valueStack.Count() >= 2 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
                    {
                        int secondVal = valueStack.Pop();

                        valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                    }

                    if (operatorStack.Count() > 0 && operatorStack.Peek() == "(")
                        operatorStack.Pop();
                    else
                        throw new ArgumentException();

                    // Same conditions as above with different operators
                    if (operatorStack.Count() > 0 && valueStack.Count() >= 2 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/"))
                    {
                        int secondVal = valueStack.Pop();

                        valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                    }
                }
            }
            // If there is 1 value left and no operators, return the last value
            if (operatorStack.Count == 0 && valueStack.Count == 1)
            {
                return valueStack.Pop();
            }

            // If there are two more values in the stack and one more operation, compute then return
            if (operatorStack.Count == 1 && valueStack.Count == 2 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
            {
                int secondVal = valueStack.Pop();

                return (Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
            }
            else throw new ArgumentException("Invalid Input");
        }

        /// <summary>
        /// This simple compute method will calculate the value of two integers
        /// with a given operation. Attempts to divide by 0 throws an exception
        /// </summary>
        /// <param name="leftNum">Integer that is placed left of the operand</param>
        /// <param name="rightNum">Integer that is placed right of the operand</param>
        /// <param name="operation">The operation to be done on the two values</param>
        /// <returns> resultant of the operation done on the left and right 
        /// integers </returns>
        /// <exception cref="DivideByZeroException"> exception thrown when
        /// dividing by 0</exception>
        /// <exception cref="ArgumentException"> exception thrown if operand
        /// is unrecognized</exception>
        private static int Compute(int leftNum, int rightNum, string operation)
        {
            if (operation == "*")
                return leftNum * rightNum;

            if (operation == "/")
            {
                if (rightNum == 0)
                    throw new ArgumentException("Cannot divide by 0");
                else
                    return leftNum / rightNum;
            }

            if (operation == "+")
                return leftNum + rightNum;

            if (operation == "-")
                return leftNum - rightNum;

            // If operation is none of the above, then throw exception
            else throw new ArgumentException("unrecognized operand");
        }
    }
}