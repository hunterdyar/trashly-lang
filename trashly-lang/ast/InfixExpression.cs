using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class InfixExpression : Expression
{
	public Expression left;
	public Expression right;
	//string? optype?
	public string Operator;
	
	public InfixExpression(Token token) : base(token)
	{
	}

	public override string ToString()
	{ return $"({Operator} {left.ToString()} {right.ToString()})";
	}
}