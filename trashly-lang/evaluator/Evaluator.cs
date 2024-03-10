using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TrashlyLang.ast;
using TrashlyLang.objects;
using Object = TrashlyLang.objects.Object;
using Boolean = TrashlyLang.objects.Boolean;
namespace TrashlyLang.evaluator;

public class Evaluator
{
	public static Null nully = new Null();
	//I feel like I should just put this in the AST. It makes the most sense? I think? //Statements eval and return... void?
	//but.... I want the code seperated? AST as a thing that gets consumed or converted, not a thing that IS the program feels better to me.
	//or at least more clear for the purpose of teaching.
	//so, what? partial classes? bleh.
	public Object Eval(Node node)
	{
		if (node is IntegerLiteral il)
		{
			return Integer.FromString(il.Token.Literal);
		}else if (node is BooleanLiteral bl)
		{
			return Boolean.FromType(bl.Token.Type);
		}else if (node is EmptyExpression)
		{
			return nully;
		}else if (node is BlockStatement bs)
		{
			Object result = new Null();
			foreach (var e in bs.Statements)
			{
				result = Eval(e);
			}

			return result;
		}else if(node is InfixExpression ife)
		{
			return EvaluateInfix(ife);
		}else if (node is GroupExpression gr)
		{
			Object result = nully;
			foreach (var child in gr.Children)
			{
				result = Eval(child);
			}

			return result;
		}else if (node is PrefixExpression pfe)
		{
			return EvaluatePrefix(pfe);
		}

		return nully;
	}

	private Object EvaluatePrefix(PrefixExpression pfe)
	{
		var right = Eval(pfe.right);
		switch (pfe.Operator)
		{
			case "!":
				return BoolMath.Negate(right);
			case "-":
				return Math.Negate(right);
		}

		throw new Exception($"I can't do prefix op on {pfe.Operator}");
	}

	private Object EvaluateInfix(InfixExpression ife)
	{
		var left = Eval(ife.left);
		var right = Eval(ife.right);
		switch (ife.Operator)
		{
			case "+":
				return Math.Sum(left, right);
			case "*":
				return Math.Multiply(left, right);
		}

		throw new Exception($"I can't do {ife.Operator} operator yet.");
		return nully;
	}
}