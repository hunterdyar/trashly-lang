using TrashlyLang.lexer;

namespace TrashlyLang.ast;

//leaf
public class Identifier : Expression
{
	public Expression Value;
	public Identifier(Token token) : base(token)
	{
	}
}