// See https://aka.ms/new-console-template for more information
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
    Console.WriteLine(Evaluator.Evaluate("4(11 + 2) + 9(21)", null));
}
catch (ArgumentException)
{
    Console.WriteLine("(4(11) + 9(21) is INVALID");
}

// Test variables
Console.WriteLine($"(10 + X1 / 5 + 3) = {Evaluator.Evaluate("(10 + X1 / 5 + 3)", var)}");


int var (string X1)
{
    return 16;
}

