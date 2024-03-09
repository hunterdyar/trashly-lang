using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class PrefixExpression : Expression
{
	public string Operator;
	public Expression right;

	public PrefixExpression(Token token) : base(token)
	{
	}

	public override string ToString()
	{
		return $"({Operator} {right.ToString()}";
	}
}