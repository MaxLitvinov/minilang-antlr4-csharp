using System.Globalization;

namespace MiniLangApp;

public class EvalVisitor : MiniLangBaseVisitor<object?>
{
    private readonly Dictionary<string, object?> _vars = new();

    public override object? VisitProgram(MiniLangParser.ProgramContext ctx)
    {
        foreach (var s in ctx.statement()) Visit(s);
        return null;
    }

    public override object? VisitVarDecl(MiniLangParser.VarDeclContext ctx)
    {
        _vars[ctx.ID().GetText()] = Visit(ctx.expr());
        return null;
    }

    public override object? VisitAssign(MiniLangParser.AssignContext ctx)
    {
        var name = ctx.ID().GetText();
        if (!_vars.ContainsKey(name))
            throw new Exception($"Змінну '{name}' не оголошено (рядок {ctx.Start.Line})");
        _vars[name] = Visit(ctx.expr());
        return null;
    }

    public override object? VisitPrint(MiniLangParser.PrintContext ctx)
    {
        var v = Visit(ctx.expr());
        Console.WriteLine(v switch
        {
            double d => d.ToString(CultureInfo.InvariantCulture),
            bool b => b ? "true" : "false",
            _ => v?.ToString()
        });
        return null;
    }

    public override object? VisitIf(MiniLangParser.IfContext ctx)
    {
        if (IsTrue(Visit(ctx.expr()))) Visit(ctx.statement(0));
        else if (ctx.statement().Length > 1) Visit(ctx.statement(1));
        return null;
    }

    public override object? VisitWhile(MiniLangParser.WhileContext ctx)
    {
        while (IsTrue(Visit(ctx.expr()))) Visit(ctx.statement());
        return null;
    }

    public override object? VisitBlock(MiniLangParser.BlockContext ctx)
    {
        foreach (var s in ctx.statement()) Visit(s);
        return null;
    }

    public override object? VisitNeg(MiniLangParser.NegContext ctx)
        => -(double)Visit(ctx.expr())!;

    public override object? VisitParens(MiniLangParser.ParensContext ctx)
        => Visit(ctx.expr());

    public override object? VisitMulDiv(MiniLangParser.MulDivContext ctx)
    {
        var l = (double)Visit(ctx.expr(0))!;
        var r = (double)Visit(ctx.expr(1))!;
        return ctx.op.Text switch
        {
            "*" => l * r,
            "/" => r == 0 ? throw new DivideByZeroException("Ділення на нуль") : l / r,
            _   => l % r
        };
    }

    public override object? VisitAddSub(MiniLangParser.AddSubContext ctx)
    {
        var l = Visit(ctx.expr(0));
        var r = Visit(ctx.expr(1));
        if (ctx.op.Text == "+" && (l is string || r is string))
            return Str(l) + Str(r);
        var a = (double)l!; var b = (double)r!;
        return ctx.op.Text == "+" ? a + b : a - b;
    }

    public override object? VisitCompare(MiniLangParser.CompareContext ctx)
    {
        var l = Visit(ctx.expr(0));
        var r = Visit(ctx.expr(1));
        switch (ctx.op.Text)
        {
            case "==": return Equals(l, r);
            case "!=": return !Equals(l, r);
        }
        var a = (double)l!; var b = (double)r!;
        return ctx.op.Text switch
        {
            "<" => a < b, "<=" => a <= b, ">" => a > b, _ => a >= b
        };
    }

    public override object? VisitNum(MiniLangParser.NumContext ctx)
        => double.Parse(ctx.GetText(), CultureInfo.InvariantCulture);

    public override object? VisitStr(MiniLangParser.StrContext ctx)
    {
        var t = ctx.GetText();
        return t.Substring(1, t.Length - 2);
    }

    public override object? VisitVar(MiniLangParser.VarContext ctx)
    {
        var name = ctx.ID().GetText();
        if (!_vars.TryGetValue(name, out var v))
            throw new Exception($"Змінну '{name}' не оголошено (рядок {ctx.Start.Line})");
        return v;
    }

    private static bool IsTrue(object? v) => v switch
    {
        bool b => b,
        double d => d != 0,
        _ => v != null
    };

    private static string Str(object? v) => v is double d
        ? d.ToString(CultureInfo.InvariantCulture)
        : v?.ToString() ?? "";
}