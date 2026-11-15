using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MiniLangApp;

Console.OutputEncoding = System.Text.Encoding.UTF8;

if (args.Length < 1)
{
    Console.WriteLine("Використання: MiniLangApp <файл.ml> [--tree]");
    return 1;
}

var input = new AntlrFileStream(args[0]);
var lexer = new MiniLangLexer(input);
var tokens = new CommonTokenStream(lexer);
var parser = new MiniLangParser(tokens);

var tree = parser.program();

if (parser.NumberOfSyntaxErrors > 0)
{
    Console.Error.WriteLine($"Знайдено синтаксичних помилок: {parser.NumberOfSyntaxErrors}");
    return 2;
}

if (args.Contains("--tree"))
    Console.WriteLine(tree.ToStringTree(parser));

try
{
    new EvalVisitor().Visit(tree);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Помилка виконання: {ex.Message}");
    return 3;
}
return 0;