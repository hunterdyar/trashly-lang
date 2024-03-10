using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using TrashlyLang.ast;
using TrashlyLang.lexer;
using TrashlyLang.memory;
using TrashlyLang.objects;
using Object = TrashlyLang.objects.Object;
using Boolean = TrashlyLang.objects.Boolean;
using Environment = TrashlyLang.memory.Environment;
using Expression = TrashlyLang.ast.Expression;

namespace TrashlyLang.evaluator;

public class Evaluator
{
	private Environment _environment;
	public Memory Memory => _environment.Memory;
	private List<Error> _errors = new List<Error>();
	public static Null nully = new Null();
	public Evaluator()
	{
		
	}

	//I feel like I should just put this in the AST. It makes the most sense? I think? //Statements eval and return... void?
	//but.... I want the code seperated? AST as a thing that gets consumed or converted, not a thing that IS the program feels better to me.
	//or at least more clear for the purpose of teaching.
	//so, what? partial classes? bleh.
	public Object EvaluateProgram(Parser.Parser parser, Environment env)
	{
		_environment = env;
		Object result = nully;
		foreach (var statement in parser.Program)
        {
        	result = Eval(statement, env);
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
	//I chose to pass Eval around everywhere because it made more sense to me (to read) than a reference variable in the evaluator.
	//but, upon reflection, this was stupid.
	public Object Eval(Node node, Environment env)
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
			var val = Eval(rs.ReturnValue, env);
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
				result = Eval(e, env);
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
			return EvaluateInfix(inx, env);
		}
		else if (node is GroupExpression gr)
		{
			Object result = nully;
			foreach (var child in gr.Children)
			{
				result = Eval(child, env);
			}

			return result;
		}
		else if (node is PrefixExpression pfe)
		{
			return EvaluatePrefix(pfe, env);
		}
		else if (node is IfExpression ife)
		{
			var condition = Eval(ife.Condition, env);
			if (IsTruthy(condition))
			{
				return Eval(ife.Consequence, env);
			}
			else
			{
				if (ife.HasAlt)
				{
					return Eval(ife.Alternative, env);
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
		}else if (node is LetStatement ls)
		{
			env.Set(ls.Identifier.Identity, Eval(ls.Value, env));
		}else if (node is Identifier ident)
		{
			return env.Get(ident.Identity);
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
			return i.Value > 0;
		}
		throw new Exception($"I can't evaluate {o} to be truthy or falsey, it should be a bool or an int.");
	}
	private Object EvaluatePrefix(PrefixExpression pfe, Environment env)
	{
		var right = Eval(pfe.right, env);
		switch (pfe.Operator)
		{
			case "!":
				return BoolMath.Negate(right);
			case "-":
				return Math.Negate(right);
		}
		return new Error($"I can't do prefix op on {pfe.Operator} {pfe.right.Token.Type}");
	}

	private Object EvaluateInfix(InfixExpression ife, Environment env)
	{
		var left = Eval(ife.left, env);
		var right = Eval(ife.right, env);
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