// See https://aka.ms/new-console-template for more information

using Expression.Parser;

List<string> expressionList = new()
{ "1-1",
  "1 -1",
  "1- 1",
  "1 - 1",
  "1- -1",
  "1 - -1",
  "1--1",
  "6 + -(4)",
  "6 + -( -4)",
  "(2 / (2 + 3.33) * 4) - -6"
};

expressionList.ForEach(expression =>
{
    Console.WriteLine($"expression: {expression}");
    Parser parser = new(expression);
    Console.WriteLine("Result: " + parser.CalculateExpression());
});

Console.WriteLine("\n Press any button");
Console.ReadLine();
