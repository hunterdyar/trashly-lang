using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class BooleanLiteral : Expression
{
	private string literal;
	public BooleanLiteral(Token token) : base(token)
	{
		literal = token.Literal;
	}

	public override string ToString()
	{
		return literal;
	}
}