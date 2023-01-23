using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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

            // Create regular expression outside the "for loop"
            string strRegex = @"[a-zA-z]+\d+";
            Regex var = new Regex(strRegex);

            for (int i = 0; i < substrings.Length; i++)
            {
                try
                {
                    // If the value is an integer... 
                    if (int.TryParse(substrings[i], out int integer))
                    {
                        if (operatorStack.Count() > 0 && operatorStack.Peek() == "*" || operatorStack.Count() > 0 && operatorStack.Peek() == "/")
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
                        if (operatorStack.Count() > 0 && operatorStack.Peek() == "*" || operatorStack.Count() > 0 && operatorStack.Peek() == "/")
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
                        if (operatorStack.Count() > 0 && operatorStack.Peek() == "+" || operatorStack.Count() > 0 && operatorStack.Peek() == "-")
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
                        if (operatorStack.Count() > 0 && operatorStack.Peek() == "+" || operatorStack.Count() > 0 && operatorStack.Peek() == "-")
                        {
                            int secondVal = valueStack.Pop();

                            valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                        }

                        operatorStack.Pop();

                        if (operatorStack.Count() > 0 && operatorStack.Peek() == "*" || operatorStack.Count() > 0 && operatorStack.Peek() == "/")
                        {
                            int secondVal = valueStack.Pop();

                            valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Invalid input", e);
                }

            }
            if (operatorStack.Count == 0)
            {
                return valueStack.Pop();    
            }

            if (operatorStack.Count != 0)
            {
                int secondVal = valueStack.Pop();

                return (Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
            }
            else throw new ArgumentException();
        }

        public static int Compute (int leftNum, int rightNum, string operation)
        {
            if (operation == "*")
                return leftNum * rightNum;

            if (operation == "/") 
            {
                if (rightNum == 0)
                    throw new DivideByZeroException();
                else
                    return leftNum / rightNum;
            }

            if (operation == "+")
                return leftNum + rightNum;

            if (operation == "-")
                return leftNum - rightNum;

            // If operation is none of the above, then throw exception
            else throw new ArgumentException();
        }
    }
}