using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class StringLiteral : Expression
{
	public string Literal;
	public StringLiteral(Token token) : base(token)
	{
		Literal = token.Literal;
	}

	public override string ToString()
	{
		return Literal;
	}
}