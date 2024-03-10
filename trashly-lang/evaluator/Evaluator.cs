using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TrashlyLang.ast;
using TrashlyLang.lexer;
using TrashlyLang.memory;
using TrashlyLang.objects;
using Object = TrashlyLang.objects.Object;
using Boolean = TrashlyLang.objects.Boolean;
using Expression = TrashlyLang.ast.Expression;

namespace TrashlyLang.evaluator;

public class Evaluator
{
	public Memory Memory;
	private List<Error> _errors = new List<Error>();
	public static Null nully = new Null();
	public Evaluator(Memory memory)
	{
		Memory = memory;
	}

	//I feel like I should just put this in the AST. It makes the most sense? I think? //Statements eval and return... void?
	//but.... I want the code seperated? AST as a thing that gets consumed or converted, not a thing that IS the program feels better to me.
	//or at least more clear for the purpose of teaching.
	//so, what? partial classes? bleh.
	public Object EvaluateProgram(Parser.Parser parser)
	{
		Object result = nully;
		foreach (var statement in parser.Program)
        {
        	result = Eval(statement);
	        //stop and return on returns and errors.
	        if (result is ReturnObject ro)
	        {
		        return ro.Value;
	        }else if (result is Error err)
	        {
		        return err;
	        }
        }

		return result;
	}
	public Object Eval(Node node)
	{
		if (node is IntegerLiteral il)
		{
			return Integer.FromString(il.Token.Literal);
		}
		else if (node is BooleanLiteral bl)
		{
			return Boolean.FromType(bl.Token.Type);
		}else if (node is ReturnStatement rs)
		{
			//wrap the object in a returnobject.
			var val = Eval(rs.ReturnValue);
			return new ReturnObject(val);
		}
		else if (node is EmptyExpression)
		{
			return nully;
		}
		else if (node is BlockStatement bs)
		{
			Object result = new Null();
			foreach (var e in bs.Statements)
			{
				result = Eval(e);
				if (result is ReturnObject ro)
				{
					//break the foreach here! we stop evaluating when we meet a return.
					return ro;
				}
			}

			return result;
		}
		else if (node is InfixExpression inx)
		{
			return EvaluateInfix(inx);
		}
		else if (node is GroupExpression gr)
		{
			Object result = nully;
			foreach (var child in gr.Children)
			{
				result = Eval(child);
			}

			return result;
		}
		else if (node is PrefixExpression pfe)
		{
			return EvaluatePrefix(pfe);
		}
		else if (node is IfExpression ife)
		{
			var condition = Eval(ife.Condition);
			if (IsTruthy(condition))
			{
				return Eval(ife.Consequence);
			}
			else
			{
				if (ife.HasAlt)
				{
					return Eval(ife.Alternative);
				}
				//if no alt, then we are done here.
			}
		}else if (node is FunctionLiteral fl)
		{
			//register function
		}else if (node is CallExpression ce)
		{
			//get registered function
			//pass arguments in
			//call
		}

		return nully;
	}

	public bool IsTruthy(objects.Object o)
	{
		if (o is Boolean b)
		{
			return b.Value;
		}
		else if (o is Integer i)
		{
			//this is cursed. Positive numbers are true, negative numbers are false. WHY.
			//i'm not even sure this is consistent?
			return i.value > 0;
		}
		throw new Exception($"I can't evaluate {o} to be truthy or falsey, it should be a bool or an int.");
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
		return new Error($"I can't do prefix op on {pfe.Operator} {pfe.right.Token.Type}");
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

		return new Error($"I can't do {ife.left.Token.Type} {ife.Operator} {ife.right.Token.Type} yet.");
	}
}