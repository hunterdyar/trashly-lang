using TrashlyLang.lexer;

namespace TrashlyLang.ast;

public class LetStatement : Node
{
	public Identifier Identifier;
	public Expression Value;
	public LetStatement(Token token) : base(token)
	{
	}
}