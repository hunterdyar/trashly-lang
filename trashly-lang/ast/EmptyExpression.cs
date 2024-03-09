using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class EmptyExpression : Expression
{
	public EmptyExpression(Token token) : base(token)
	{
	}

	public override string ToString()
	{
		return "(empty)";
	}
}