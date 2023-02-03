// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
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
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        // Create fields for the list of tokens and set of normalized values
        private List<String> tokens;
        private HashSet<String> normalized;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
        this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            // Move all the contents of the input string to a list
            tokens = new List<string>(GetTokens(formula));

            // if contents of the list are empty, immediately throw
            if (tokens.Count == 0)
            {
                throw new FormulaFormatException("Formula can not be empty. Try adding elements.");
            }
            // Create list to store normalized values
            normalized = new HashSet<string>();

            // If the first item is an operator or closing parenthesis or the last item is an operator or
            // opening parenthesis, throw an exception 
            if (hasValidOp(tokens[0]) || tokens[0] == ")" || hasValidOp(tokens[tokens.Count - 1]) 
                || tokens[tokens.Count - 1] == "(")
            {
                throw new FormulaFormatException("Formula can not begin with an operator or closing parenthesis or end with " +
                                                 "an operator and opening parenthesis");
            }

            // Create string regex for valid variable input: a letter or underscore followed by zero or more letters,
            // underscores, or digits
            string validVar = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            Regex varPattern = new Regex(validVar);

            // Keep track of number of opening and closing parenthesis
            int openPar = 0;
            int closePar = 0;

            // Keep track of the index of the previous token
            int prevToken = 0;

            for (int i = 0; i < tokens.Count; i++)
            {
                // If token is a open parenthesis
                if (tokens[i] == "(")
                {
                    // If this is not the first iteration, then check if previous will throw
                    if (i != 0)
                    {
                        // If the previous token is a number or a variable
                        if (double.TryParse(tokens[prevToken], out _) || varPattern.IsMatch(tokens[prevToken]))
                        {
                            throw new FormulaFormatException("There must be operations between values/variables and parenthesis ");
                        }
                    }
                    openPar++;
                    prevToken = i;
                }

                // If token is a closing parenthesis
                if (tokens[i] == ")")
                {
                    // If this is not the first iteration, then check if previous will throw
                    if (i != 0)
                    {
                        // If the previous token is an opening parenthesis, throw
                        if (tokens[prevToken] == "(")
                        {
                            throw new FormulaFormatException("Must have a value between the parenthesis");
                        }
                    }
                    closePar++;
                    prevToken = i;
                }

                // If token is a valid number
                if (double.TryParse(tokens[i], out _))
                {
                    // If this is not the first iteration, then check if previous will throw
                    if (i != 0)
                    {
                        // If the previous value is anything other than an operand or opening parenthesis, then throw exception
                        if (double.TryParse(tokens[prevToken], out _)
                            || tokens[prevToken] == ")" || varPattern.IsMatch(tokens[prevToken]))
                        {
                            throw new FormulaFormatException("numbers must have operators or open parenthesis preceding");
                        }
                    }
                    prevToken = i;
                }

                // If token is a valid operator
                if (hasValidOp(tokens[i]))
                {
                    // If this is not the first iteration, then check if previous will throw
                    if (i != 0)
                    {
                        // If previous token is an operator or opening parenthesis, throw
                        if (hasValidOp(tokens[prevToken]) || tokens[prevToken] == "(")
                        {
                            throw new FormulaFormatException("Formula can not have operators sequentially or open parenthesis preceding");
                        }
                    }
                    prevToken = i;
                }

                // if the token is a valid variable input and it is not a valid number
                if (varPattern.IsMatch(tokens[i]) && !double.TryParse(tokens[i], out _))
                {
                    // If the normalized token does not match the pattern, throw
                    if (!varPattern.IsMatch(normalize(tokens[i])))
                    {
                        throw new FormulaFormatException("normalized variable does not match the pattern");
                    }

                    // If the normalized token does not match validator restrictions, throw
                    if (!isValid(normalize(tokens[i])))
                    {
                        throw new FormulaFormatException("normalized variable does not comply with validator");
                    }

                    // Set the token to the normalized token 
                    tokens[i] = normalize(tokens[i]);

                    // If this is not the first iteration, then check if previous will throw
                    if (i != 0)
                    {
                        // If the previous value is anything other than an operand or opening parenthesis, then throw exception
                        if (double.TryParse(tokens[prevToken], out _) || tokens[prevToken] == ")" || varPattern.IsMatch(tokens[prevToken]))
                        {
                            throw new FormulaFormatException("Variables must have operators or open parenthesis preceding");
                        }
                    }
                    normalized.Add(tokens[i]);
                    prevToken = i; ;
                }
            }

            // Number of open parentheses must equal the number of closed ones
            if (openPar != closePar)
            { 
                throw new FormulaFormatException("Each opening parenthesis must contain a closing one. Check if you are missing a parenthesis.");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {

            Stack<double> valueStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();

            // Create regular expression outside the for loop
            string strRegex = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            Regex var = new Regex(strRegex);

            for (int i = 0; i < tokens.Count; i++)
            {
                // If the value is a double... 
                if (double.TryParse(tokens[i], out double value))
                {
                    if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/") && valueStack.Count > 0)
                    {
                        // Catch division by 0
                        if (operatorStack.Peek() == "/" && value == 0)
                        {
                            return new FormulaError("can not divide by 0");
                        }
                        valueStack.Push(Compute(valueStack.Pop(), value, operatorStack.Pop()));
                    }
                    else
                    {
                        valueStack.Push(value);
                    }
                }

                // If t is a variable...
                if (var.IsMatch(tokens[i]))
                {
                    // if variable is not a double, throw
                    double verifyDouble;
                    try
                    {
                        verifyDouble = lookup(tokens[i]);
                    }
                    catch(ArgumentException) 
                    { 
                        return new FormulaError("variable does not compute to valid number");
                    }

                    // When operator stack contains "*" or "/" and the value stack is not empty, compute value on stack, variable, and
                    // operator and push to value stack
                    if (operatorStack.Count > 0 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/") && valueStack.Count > 0)
                    {
                        // Catch division by 0
                        if (operatorStack.Peek() == "/" && lookup(tokens[i]) == 0)
                        {
                            return new FormulaError("can not divide by 0");
                        }

                        valueStack.Push(Compute(valueStack.Pop(), lookup(tokens[i]), operatorStack.Pop()));
                    }

                    else
                    {
                        valueStack.Push(lookup(tokens[i]));
                    }
                }

                // If t is + or -...
                if (tokens[i] == "+" || tokens[i] == "-")
                {
                    // If operator stacks contain "+" or "-" and value stack is 2 or greater, pop operand and both values and push compute 
                    if (operatorStack.Count > 0 && valueStack.Count >= 2 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
                    {
                        double secondVal = valueStack.Pop();                                               
                        valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));                       
                    }
                    operatorStack.Push(tokens[i]);
                }

                // If t is *, /, or (...
                if (tokens[i] == "*" || tokens[i] == "/" || tokens[i] == "(")
                {
                    operatorStack.Push(tokens[i]);
                }

                // If t is )...
                if (tokens[i] == ")")
                {
                    // If operator stacks contain "+" or "-" and value stack is 2 or greater, pop operand and both values and push compute 
                    if (operatorStack.Count() > 0 && valueStack.Count() >= 2 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
                    {
                        double secondVal = valueStack.Pop();                                                                      
                        valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));                        
                    }

                    if (operatorStack.Count() > 0 && operatorStack.Peek() == "(")
                        operatorStack.Pop();
                    else
                        return new FormulaError("Operator stack must be greater than 1 and have a closing parenthesis");

                    // Same conditions as above with different operators
                    if (operatorStack.Count() > 0 && valueStack.Count() >= 2 && (operatorStack.Peek() == "*" || operatorStack.Peek() == "/"))
                    {
                        double secondVal = valueStack.Pop();

                        // Catch divide by 0 error
                        if (operatorStack.Peek() == "/" && secondVal == 0)
                        {
                            return new FormulaError("can not divide by 0");
                        }
                        else
                        {
                            valueStack.Push(Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                        }
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
                double secondVal = valueStack.Pop();

                // Catch divide by 0 error
                if (operatorStack.Peek() == "/" && secondVal == 0)
                {
                    return new FormulaError("can not divide by 0");
                }
                else
                {
                    return (Compute(valueStack.Pop(), secondVal, operatorStack.Pop()));
                }
            }

            return new FormulaError("undefined variables");

        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            // Create and return a copy of normalized 
            HashSet<String> normalizedCopy = new HashSet<String>(normalized);
            return normalizedCopy;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string formulaString = "";
            for (int i = 0; i < tokens.Count; i++)
            {
                formulaString += tokens[i];
            }
            return formulaString;
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            // If obj is null or not a formula, return false
            if (object.Equals(obj, null) || obj.GetType() != this.GetType()) 
                return false;
            
            Formula formula = (Formula)obj;

            for (int i = 0; i < tokens.Count; i++)  
            {
                // If the item is a number, compare with conversion from string to double, then back to string
                if (double.TryParse(tokens[i], out double value))
                {
                    if (double.Parse(tokens[i]).ToString() != double.Parse(formula.tokens[i]).ToString())
                    {
                        return false;
                    }
                }
                else 
                    if (!(tokens[i].ToString().Equals(formula.tokens[i])))
                    {
                        return false;
                    }                      
            }
            return true;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return (f1.Equals(f2));         
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return (!(f1.Equals(f2)));
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = ToString().GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        
        /// <summary>
        /// Small helper method that returns true if the token is a valid operator
        /// </summary>
        /// <param name="token"></param>
        /// <returns>boolean</returns>
        private bool hasValidOp(String token)
        {
            if (token == "+" || token == "-" || token == "*" || token == "/")
            {
                return true;
            }
            else
                return false;
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
        /// <exception cref="ArgumentException"> exception thrown if operand
        /// is unrecognized, should not be thrown</exception>
        private static double Compute(double leftNum, double rightNum, string operation)
        {
            if (operation == "*")
                return leftNum * rightNum;

            if (operation == "/")
                return leftNum / rightNum;

            if (operation == "+")
                return leftNum + rightNum;

            if (operation == "-")
                return leftNum - rightNum;

            // Should never happen
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
        : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
        : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}