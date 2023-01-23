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
/// This console application runs a series of tests that ensures the 
/// methods in the library class function correctly.
/// </summary>
using FormulaEvaluator;

// Test simple addition and subtraction
Console.WriteLine($"5+3 = {Evaluator.Evaluate("5+3", null)}");
Console.WriteLine($"5-3 = {Evaluator.Evaluate("5-3", null)}");
Console.WriteLine($"436+842 = {Evaluator.Evaluate("436+842", null)}");
Console.WriteLine($"590-253 = {Evaluator.Evaluate("590-253", null)}");

// Test simple addition and subtraction with parentheses and spaces
Console.WriteLine($"(5+3) = {Evaluator.Evaluate("(5+3)", null)}");
Console.WriteLine($"(436+842) = {Evaluator.Evaluate("( 436+ 842  )", null)}");
Console.WriteLine($"(590-253) = {Evaluator.Evaluate("(590 -   253)", null)}");

// Test simple multiplication
Console.WriteLine($"(12 * 12) = {Evaluator.Evaluate("(12 * 12)", null)}");
Console.WriteLine($"(231 * 14) = {Evaluator.Evaluate("(231 * 14)", null)}");

// Test simple division
Console.WriteLine($"(56 / 7) = {Evaluator.Evaluate("(56 /7)", null)}");
Console.WriteLine($"(231 / 14) = {Evaluator.Evaluate("(231 / 14)", null)}");

// Test order of operations
Console.WriteLine($"(10 + 10 / 5 + 3) = {Evaluator.Evaluate("(10 + 10 / 5 + 3)", null)}");
Console.WriteLine($"(10 + 10 / 5 * 3) = {Evaluator.Evaluate("(10 + 10 / 5 * 3)", null)}");
Console.WriteLine($"(9+8) * 12 + (8 - 5) = {Evaluator.Evaluate("(9+8) * 12 + (8 - 5)", null)}");

// Test invalid input exceptions
try
{
    Evaluator.Evaluate("(-1 + 5)", null);
}
catch (ArgumentException)
{
    Console.WriteLine("(-1 + 5) is INVALID");
}

try
{
    Evaluator.Evaluate("(+1 + 5)", null);
}
catch (ArgumentException)
{
    Console.WriteLine("(+1 + 5) is INVALID");
}

try
{
    Evaluator.Evaluate("(/1 + 8 - 5)", null);
}
catch (ArgumentException)
{
    Console.WriteLine("(/1 + 8 - 5) is INVALID");
}

try
{
    Evaluator.Evaluate("(*1 ++ 5)", null);
}
catch (ArgumentException)
{
    Console.WriteLine("(*1 ++ 5) is INVALID");
}

try
{
    Evaluator.Evaluate("(11 + 4 5 2)", null);
}
catch (ArgumentException)
{
    Console.WriteLine("(11 + 4 5 2) is INVALID");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("(6)(12)", null));
}
catch (ArgumentException)
{
    Console.WriteLine("(6)(12) is INVALID");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("4(11) + 9(21)", null));
}
catch (ArgumentException)
{
    Console.WriteLine("(4(11) + 9(21) is INVALID");
}

//Test divide by 0
try
{
    Console.WriteLine(Evaluator.Evaluate("10/0", null));
}
catch (DivideByZeroException)
{
    Console.WriteLine("10/0 is INVALID");
}
try
{
    Console.WriteLine(Evaluator.Evaluate("100 + 10 * 5 + 3/0", null));
}
catch (DivideByZeroException)
{
    Console.WriteLine("100 + 10 * 5 + 3/0 is INVALID");
}

// Test variables
Console.WriteLine($"(10 + X1 / 5 + 3) = {Evaluator.Evaluate("(10 + X1 / 5 + 3)", var)}");
Console.WriteLine($"(X1 + X1 / 5 + X1) = {Evaluator.Evaluate("(X1 + X1 / 5 + X1)", var)}");
Console.WriteLine($"(Xxx2341 + Xxx2341 / 3 + Xxx2341) = {Evaluator.Evaluate("(Xxx2341 + Xxx2341 / 3 + Xxx2341)", var2)}");
Console.WriteLine($"Ghsd421uy + 9 = {Evaluator.Evaluate("Ghsd421uy + 9", var4)}");

// Test invalid variables
try
{
    Console.WriteLine(Evaluator.Evaluate("A + 9", var3));
}
catch (ArgumentException)
{
    Console.WriteLine("A + 9 is INVALID");
}

try
{
    Console.WriteLine(Evaluator.Evaluate("G + 9", var5));
}
catch (ArgumentException)
{
    Console.WriteLine("G + 9 is INVALID");
}

int var (string X1)
{
    return 16;
}

int var2(string Xxx2341)
{
    return 9;
}

int var3(string A)
{
    return 8;
}

int var4(string Ghsd421uy)
{
    return 8;
}

int var5(string A)
{
    return -10;
}

