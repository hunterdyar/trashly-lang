using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class Boolean : Expression
{
	private string literal;
	public Boolean(Token token) : base(token)
	{
		literal = token.Literal;
	}

	public override string ToString()
	{
		return literal;
	}
}