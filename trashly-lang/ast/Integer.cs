using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class Integer : Expression
{
	private string literal;
	public Integer(Token token) : base(token)
	{
		literal = token.Literal;
	}

	public override string ToString()
	{
		return literal;
	}
}