using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TrashlyLang.ast;
using TrashlyLang.lexer;
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
		}else if(node is InfixExpression inx)
		{
			return EvaluateInfix(inx);
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
		}else if (node is IfExpression ife)
		{
			var condition = Eval(ife.Condition);
			if (condition is Boolean b)
			{
				if (b.Value)
				{
					return Eval(ife.Consequence);
				}
				else
				{
					if (ife.HasAlt)
					{
						return Eval(ife.Alternative);
					}
				}
			}
			else
			{
				throw new Exception("if statements need conditional in the (), and this dont that?");
			}

			return nully;
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
			case "-":
				return Math.Subtract(left, right);
			case "*":
				return Math.Multiply(left, right);
			case "/":
				return Math.Divide(left, right);
			case "<":
				return BoolMath.Compare(TokenType.LessThan, left, right);
			case ">":
				return BoolMath.Compare(TokenType.GreaterThan, left, right);
			case "==":
				return BoolMath.Compare(TokenType.Equals, left, right);
			case "!=":
				return BoolMath.Compare(TokenType.NotEqual, left, right);
		}

		throw new Exception($"I can't do {ife.Operator} operator yet.");
		return nully;
	}
}